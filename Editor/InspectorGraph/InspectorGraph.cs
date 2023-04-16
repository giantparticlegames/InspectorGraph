// ********************************
// (C) 2022 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using System.Collections.Generic;
using GiantParticle.InspectorGraph.Editor.UIDocuments;
using GiantParticle.InspectorGraph.Editor.Data;
using GiantParticle.InspectorGraph.Editor.Data.Nodes;
using GiantParticle.InspectorGraph.Editor.Data.Nodes.Filters;
using GiantParticle.InspectorGraph.Editor.Manipulators;
using GiantParticle.InspectorGraph.Editor.Preferences;
using GiantParticle.InspectorGraph.Editor.Settings;
using GiantParticle.InspectorGraph.Editor.Views;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;


namespace GiantParticle.InspectorGraph.Editor
{
    internal class InspectorGraph : EditorWindow
    {
        private const int kPositionXOffset = 80;
        private const int kPositionYOffset = 30;

        private ContentViewRegistry _viewRegistry = new();
        private HashSet<InspectorWindow> _waitForResize = new();
        private VisualElement _windowView;
        private VisualElement _content;
        private InspectorGraphToolbar _toolbar;

        private ReferenceNodeFactory _nodeFactory;
        private IObjectNode _rootNode;

        [MenuItem("Window/Giant Particle/Inspector Graph")]
        public static void ShowWindow()
        {
            EditorWindow window = GetWindow<InspectorGraph>();
            window.titleContent = new GUIContent("Inspector Graph");
        }

        public void OnDestroy()
        {
            ClearCurrentContent();
            _toolbar.Dispose();
            GlobalApplicationContext.Dispose();
        }

        public void OnEnable()
        {
            GlobalApplicationContext.Instantiate();
            IApplicationContext context = GlobalApplicationContext.Instance;
            if (!context.Contains<IPreferenceHandler>())
            {
                var handler = new PreferenceHandler();
                handler.LoadAllPreferences();
                context.Add<IPreferenceHandler>(handler);
            }

            if (!context.Contains<IInspectorGraphSettings>())
                context.Add<IInspectorGraphSettings>(InspectorGraphSettings.GetSettings());

            if (!context.Contains<ITypeFilterHandler>())
            {
                var settings = context.Get<IInspectorGraphSettings>();
                context.Add<ITypeFilterHandler>(new TypeFilterHandler(settings));
            }

            if (_nodeFactory == null)
            {
                var typeFilterHandler = context.Get<ITypeFilterHandler>();
                _nodeFactory = new ReferenceNodeFactory(typeFilterHandler);
            }

            if (!context.Contains<IContentViewRegistry>())
                context.Add<IContentViewRegistry>(_viewRegistry);

            // UI Catalogs
            if (!context.Contains<IUIDocumentCatalog<MainWindowUIDocumentType>>())
                context.Add<IUIDocumentCatalog<MainWindowUIDocumentType>>(MainWindowUIDocumentCatalog.GetCatalog());
            if (!context.Contains<IUIDocumentCatalog<InspectorWindowUIDocumentType>>())
                context.Add<IUIDocumentCatalog<InspectorWindowUIDocumentType>>(InspectorWindowUIDocumentCatalog
                    .GetCatalog());
        }

        public void OnDisable()
        {
            ClearCurrentContent();
        }

        public void CreateGUI()
        {
            var catalog = GlobalApplicationContext.Instance.Get<IUIDocumentCatalog<MainWindowUIDocumentType>>();
            var layout = catalog[MainWindowUIDocumentType.MainWindow].Asset;
            layout.CloneTree(rootVisualElement);

            _windowView = rootVisualElement.Q<VisualElement>(nameof(_windowView));
            _content = rootVisualElement.Q<VisualElement>(nameof(_content));

            _toolbar = new InspectorGraphToolbar(new InspectorGraphToolbarConfig()
            {
                CreateCallback = CreateContentTree, ResetCallback = ResetView, UpdateCallback = UpdateView
            });
            var toolbarContainer = rootVisualElement.Q<VisualElement>("_toolbarContainer");
            toolbarContainer.Add(_toolbar);

            var zoomController = new ContentZoomController(_content);
            var footer = rootVisualElement.Q<Toolbar>("_footer");
            footer.Add(zoomController);
            zoomController.ZoomLevelChanged += element => UpdateWindowVisibility();

            var moveManipulator = new DragManipulator(_windowView, _content,
                new[]
                {
                    new ActivatorCombination(ManipulatorButton.Middle),
                    Application.platform == RuntimePlatform.OSXEditor
                        ? new ActivatorCombination(ManipulatorButton.Left,
                            EventModifiers.Alt | EventModifiers.Command)
                        : new ActivatorCombination(ManipulatorButton.Left,
                            EventModifiers.Alt | EventModifiers.Control)
                });
            moveManipulator.PositionChanged += element => UpdateWindowVisibility();

            _toolbar.LoadPreferences();
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

        private void ResetView()
        {
            if (_rootNode == null) return;
            _content.transform.position = Vector3.zero;
            CreateContentTree(_rootNode.Target);
        }

        private void CreateContentTree(Object referenceObject)
        {
            _content.transform.position = Vector3.zero;
            ClearCurrentContent();
            UpdateView(referenceObject);
        }

        private void UpdateView()
        {
            if (_rootNode == null) return;
            UpdateView(_rootNode.Target);
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
            var settings = GlobalApplicationContext.Instance.Get<IInspectorGraphSettings>();
            var key = node.Target;
            if (_viewRegistry.IsWindowRegisteredByTarget(key)) return null;
            if (_viewRegistry.WindowCount > settings.MaxWindows)
            {
                Debug.LogWarning("Max Window limit reached");
                return null;
            }

            var window = new InspectorWindow(
                node: node,
                forceStaticPreview: _viewRegistry.WindowCount > settings.MaxPreviewWindows);
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
                    if (targetWindow == null) continue;
                    if (!_viewRegistry.ContainsConnection(sourceWindow, targetWindow))
                    {
                        var line = new ConnectionLine(source: sourceWindow, dest: targetWindow, nodeReference.RefType);
                        _content.Add(line);
                        line.SendToBack();
                        // Register line
                        _viewRegistry.RegisterConnection(line);
                    }

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

            List<float> maxWidthPerLevel = new List<float>();
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
                if (window == null) continue;
                // Avoid reposition already repositioned window
                if (windowsVisited.Contains(window)) continue;

                // Store max width
                if (maxWidthPerLevel.Count > level)
                    maxWidthPerLevel[level] = Math.Max(maxWidthPerLevel[level], window.contentRect.width);
                else
                    maxWidthPerLevel.Add(window.contentRect.width);

                // Avoid moving manually moved window
                if (!window.Node.WindowData.HasBeenManuallyMoved)
                {
                    float newPositionX = 0;
                    for (int i = 0; i < level; ++i)
                        newPositionX += maxWidthPerLevel[i] + kPositionXOffset;

                    window.transform.position = new Vector3(
                        x: newPositionX,
                        y: currentY,
                        z: 0);
                }

                currentY += window.contentRect.height + kPositionYOffset;
                windowsVisited.Add(window);
            }

            _content.ResizeToFit<InspectorWindow>();
            UpdateWindowVisibility();
        }

        private void OnInspectorWindowMoved(VisualElement window)
        {
            if (window is InspectorWindow inspectorWindow)
                inspectorWindow.Node.WindowData.HasBeenManuallyMoved = true;
            _content.ResizeToFit<InspectorWindow>();
        }

        private void UpdateWindowVisibility()
        {
            _viewRegistry.ExecuteOnEachWindow(window =>
            {
                window.visible = _windowView.worldBound.Overlaps(window.worldBound);
            });
        }
    }
}
