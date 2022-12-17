// ********************************
// (C) 2022 - Giant Particle Games 
// All rights reserved.
// ********************************

using System;
using System.Collections.Generic;
using GiantParticle.InspectorGraph.Editor.Common;
using GiantParticle.InspectorGraph.Editor.Common.Manipulators;
using GiantParticle.InspectorGraph.Editor.MultiInspector.Data.Nodes;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;


namespace GiantParticle.InspectorGraph
{
    public class InspectorGraph : EditorWindow
    {
        private const int kPositionXOffset = 80;
        private const int kPositionYOffset = 30;
        private const int kDefaultWindowMaxWidth = 400;
        private const int kDefaultWindowMaxHeight = 300;

        public VisualTreeAsset WindowVisualTree;
        public VisualTreeAsset InspectorWindowVisualTree;

        private ContentViewRegistry _viewRegistry = new();
        private HashSet<InspectorWindow> _waitForResize = new();
        private ScrollView _content;
        private ObjectField _refField;

        private ReferenceNodeFactory _nodeFactory = new ReferenceNodeFactory();
        private IObjectNode _rootNode;
        private Preferences _preferences;

        [MenuItem("Giant Particle/Editor Tools/Inspector Graph")]
        public static void ShowWindow()
        {
            EditorWindow window = GetWindow<InspectorGraph>();
            window.titleContent = new GUIContent("Inspector Graph");
        }

        public void OnDestroy()
        {
            ClearCurrentContent();
            GlobalApplicationContext.Dispose();
        }

        public void OnDisable()
        {
            ClearCurrentContent();
        }

        public void CreateGUI()
        {
            GlobalApplicationContext.Instantiate();
            GlobalApplicationContext.Instance.Add<IContentViewRegistry>(_viewRegistry);
            WindowVisualTree.CloneTree(rootVisualElement);

            _content = rootVisualElement.Q<ScrollView>(nameof(_content));

            // Toolbar
            var viewMenu = rootVisualElement.Q<ToolbarMenu>("_viewMenu");
            viewMenu.menu.AppendAction(
                actionName: "Refresh",
                action: (menuAction) =>
                {
                    if (_rootNode == null) return;
                    UpdateView(_rootNode.Target);
                });
            viewMenu.menu.AppendSeparator();
            viewMenu.menu.AppendAction(
                actionName: "Show/Scripts",
                action: (menuAction) =>
                {
                    Type monoScriptType = typeof(MonoScript);
                    if (_nodeFactory.ExcludedTypes.Contains(monoScriptType))
                        _nodeFactory.ExcludedTypes.Remove(monoScriptType);
                    else _nodeFactory.ExcludedTypes.Add(monoScriptType);
                    // Refresh view if needed
                    if (_rootNode != null) UpdateView(_rootNode.Target);
                },
                actionStatusCallback: action =>
                {
                    if (!_nodeFactory.ExcludedTypes.Contains(typeof(MonoScript)))
                        return DropdownMenuAction.Status.Checked;
                    return DropdownMenuAction.Status.Normal;
                });
            viewMenu.menu.AppendAction(
                actionName: "Show/Prefab References",
                action: (menuAction) =>
                {
                    _nodeFactory.ShouldProcessPrefabs = !_nodeFactory.ShouldProcessPrefabs;
                    // Refresh view if needed
                    if (_rootNode != null) UpdateView(_rootNode.Target);
                },
                actionStatusCallback: action => _nodeFactory.ShouldProcessPrefabs
                    ? DropdownMenuAction.Status.Checked
                    : DropdownMenuAction.Status.Normal);

            // Update Reference Field
            _refField = rootVisualElement.Q<ObjectField>(nameof(_refField));
            _refField.objectType = typeof(Object);
            _refField.RegisterCallback<ChangeEvent<Object>>(evt =>
            {
                var assignedObject = evt.newValue;
                if (assignedObject == null) _preferences.LastInspectedObjectPath = null;
                else
                {
                    var path = AssetDatabase.GetAssetPath(assignedObject);
                    _preferences.LastInspectedObjectPath = path;
                    _preferences.Save();
                }

                CreateContentTree(assignedObject);
            });

            var resetButton = rootVisualElement.Q<ToolbarButton>("_reset");
            resetButton.RegisterCallback<ClickEvent>(evt =>
            {
                if (_rootNode == null) return;
                CreateContentTree(_rootNode.Target);
            });

            var footer = rootVisualElement.Q<Toolbar>("_footer");
            footer.Add(new ContentZoomController(_content.contentContainer));

            // Load Last assigned reference
            LoadPreferences();
        }

        private void LoadPreferences()
        {
            _preferences = Preferences.LoadPreferences();
            if (!string.IsNullOrEmpty(_preferences.LastInspectedObjectPath))
            {
                var lastObject = AssetDatabase.LoadAssetAtPath<Object>(_preferences.LastInspectedObjectPath);
                if (lastObject != null)
                    _refField.value = lastObject;
            }
        }

        private void ClearCurrentContent()
        {
            _viewRegistry.ExecuteOnEachWindow((window) =>
            {
                StopObservingWindow(window);
                window.RemoveFromHierarchy();
                window.Dispose();
            });
            _viewRegistry.ExecuteOnEachConnection((connection) => { connection.RemoveFromHierarchy(); });
            _viewRegistry.Clear();

            _content?.Clear();
            _rootNode = null;
            if (GlobalApplicationContext.IsInstantiated) GlobalApplicationContext.Instance.Remove<IObjectNode>();
            _waitForResize.Clear();
        }

        private void ObserveWindow(InspectorWindow window)
        {
            if (_waitForResize.Contains(window)) return;

            // Register for Geometry changes to resize container
            _waitForResize.Add(window);
            window.RegisterCallback<GeometryChangedEvent>(OnInspectorWindowGeometryChanged);

            // Register for movement
            foreach (IManipulator manipulator in window.Manipulators)
            {
                if (manipulator is IPositionManipulator positionManipulator)
                    positionManipulator.PositionChanged += OnInspectorWindowMoved;
            }

            // Register for GUI changes
            window.GUIChanged += OnWindowGUIChanged;
        }

        private void StopObservingWindow(InspectorWindow window)
        {
            _waitForResize.Remove(window);
            window.UnregisterCallback<GeometryChangedEvent>(OnInspectorWindowGeometryChanged);

            // Deregister for movement
            foreach (IManipulator manipulator in window.Manipulators)
            {
                if (manipulator is IPositionManipulator positionManipulator)
                    positionManipulator.PositionChanged -= OnInspectorWindowMoved;
            }

            // Deregister for GUI changes
            window.GUIChanged -= OnWindowGUIChanged;
        }

        private void CreateContentTree(Object referenceObject)
        {
            ClearCurrentContent();
            UpdateView(referenceObject);
        }

        private void UpdateView(Object referenceObject)
        {
            if (referenceObject == null) return;
            _rootNode = _nodeFactory.CreateGraphFromObject(referenceObject);
            GlobalApplicationContext.Instance.Remove<IObjectNode>();
            GlobalApplicationContext.Instance.Add<IObjectNode>(_rootNode);

            Queue<IObjectNode> queue = new();
            HashSet<IObjectNode> visitedNodes = new();
            List<InspectorWindow> newWindows = new();
            queue.Enqueue(_rootNode);

            // Create windows
            while (queue.Count > 0)
            {
                var item = queue.Dequeue();
                if (visitedNodes.Contains(item)) continue;
                visitedNodes.Add(item);

                // Create Window
                var window = CreateWindow(item);
                if (window != null) newWindows.Add(window);

                // Enqueue
                foreach (IObjectNodeReference nodeReference in item.References)
                    queue.Enqueue(nodeReference.TargetNode);
            }

            DeleteLingeringWindows();

            // Draw Lines
            DrawConnections();

            // Observe newly added windows for changes in geometry and position
            for (int i = 0; i < newWindows.Count; ++i)
                ObserveWindow(newWindows[i]);

            // Update all windows
            _viewRegistry.ExecuteOnEachWindow((window) => { window.UpdateView(); });

            ReorderWindows();
        }

        private void DeleteLingeringWindows()
        {
            Queue<IObjectNode> queue = new();
            HashSet<IObjectNode> visitedNodes = new();
            queue.Enqueue(_rootNode);

            while (queue.Count > 0)
            {
                var node = queue.Dequeue();
                if (visitedNodes.Contains(node)) continue;
                visitedNodes.Add(node);

                // Enqueue
                foreach (IObjectNodeReference nodeReference in node.References)
                    queue.Enqueue(nodeReference.TargetNode);
            }

            // Check objects
            HashSet<Object> visitedObjects = new(visitedNodes.Count);
            foreach (IObjectNode visitedNode in visitedNodes)
                visitedObjects.Add(visitedNode.Target);
            List<InspectorWindow> windowsToDelete = new(_viewRegistry.AllWindowsExceptByKey(visitedObjects));

            // Delete
            for (int i = 0; i < windowsToDelete.Count; ++i)
            {
                var windowToDelete = windowsToDelete[i];
                _viewRegistry.DeregisterWindow(windowToDelete);

                // Delete Connections
                _viewRegistry.ExecuteOnEachConnectionByWindow(
                    (connection) => { connection.RemoveFromHierarchy(); },
                    windowToDelete);
                _viewRegistry.DeregisterConnectionsRelatedToWindow(windowToDelete);

                StopObservingWindow(windowToDelete);
                windowToDelete.RemoveFromHierarchy();
                windowToDelete.DisposeContent();
            }
        }

        private InspectorWindow CreateWindow(IObjectNode node)
        {
            var key = node.Target;
            if (_viewRegistry.IsWindowRegisteredByTarget(key)) return null;

            var window = new InspectorWindow(node: node, visualTreeAsset: InspectorWindowVisualTree);
            window.style.width = new StyleLength(kDefaultWindowMaxWidth);
            window.style.maxHeight = new StyleLength(kDefaultWindowMaxHeight);
            window.style.position = new StyleEnum<Position>(Position.Absolute);

            _content.Add(window);
            _viewRegistry.RegisterWindow(window);
            return window;
        }

        private void DrawConnections()
        {
            Queue<IObjectNode> queue = new();
            HashSet<IObjectNode> visitedNodes = new();
            queue.Enqueue(_rootNode);

            while (queue.Count > 0)
            {
                var node = queue.Dequeue();
                if (visitedNodes.Contains(node)) continue;
                visitedNodes.Add(node);

                var sourceWindow = _viewRegistry.WindowByTarget(node.Target);
                foreach (IObjectNodeReference nodeReference in node.References)
                {
                    var targetWindow = _viewRegistry.WindowByTarget(nodeReference.TargetNode.Target);
                    var line = new ConnectionLine(source: sourceWindow, dest: targetWindow, nodeReference.RefType);
                    _content.Add(line);
                    line.SendToBack();
                    // Register line
                    _viewRegistry.RegisterConnection(line);

                    queue.Enqueue(nodeReference.TargetNode);
                }
            }
        }

        private void OnInspectorWindowGeometryChanged(GeometryChangedEvent evt)
        {
            InspectorWindow source = (InspectorWindow)evt.target;
            source.UnregisterCallback<GeometryChangedEvent>(OnInspectorWindowGeometryChanged);
            _waitForResize.Remove(source);
            if (_waitForResize.Count > 0)
                return;

            ReorderWindows();
        }

        private void OnWindowGUIChanged()
        {
            UpdateView(_rootNode.WindowData.Target);
        }

        private void ReorderWindows()
        {
            Queue<Tuple<IObjectNode, int>> queue = new();
            queue.Enqueue(new Tuple<IObjectNode, int>(_rootNode, 0));
            HashSet<InspectorWindow> windowsVisited = new();

            float currentY = 0;
            int currentLevel = 0;
            while (queue.Count > 0)
            {
                var item = queue.Dequeue();
                IObjectNode node = item.Item1;
                int level = item.Item2;

                // Add children
                foreach (IObjectNodeReference nodeReference in node.References)
                    queue.Enqueue(new Tuple<IObjectNode, int>(nodeReference.TargetNode, level + 1));

                if (currentLevel != level)
                {
                    currentY = 0;
                    currentLevel = level;
                }

                InspectorWindow window = _viewRegistry.WindowByTarget(item.Item1.Target);
                // Avoid reposition already repositioned window
                if (windowsVisited.Contains(window)) continue;

                // Avoid moving manually moved window
                if (!window.Node.WindowData.HasBeenManuallyMoved)
                {
                    window.transform.position = new Vector3(
                        x: level * (kDefaultWindowMaxWidth + kPositionXOffset),
                        y: currentY,
                        z: 0);
                }

                currentY += window.contentRect.height + kPositionYOffset;
                windowsVisited.Add(window);
            }

            _content.contentContainer.ResizeToFit<InspectorWindow>();
        }

        private void OnInspectorWindowMoved(VisualElement window)
        {
            if (window is InspectorWindow inspectorWindow)
                inspectorWindow.Node.WindowData.HasBeenManuallyMoved = true;
            _content.contentContainer.ResizeToFit<InspectorWindow>();
        }
    }
}
