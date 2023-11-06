// ********************************
// (C) 2022 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using System.Collections.Generic;
using GiantParticle.InspectorGraph.Data;
using GiantParticle.InspectorGraph.Data.Nodes;
using GiantParticle.InspectorGraph.Data.Graph.Filters;
using GiantParticle.InspectorGraph.Editor.InspectorGraph.Data.Graph;
using GiantParticle.InspectorGraph.Manipulators;
using GiantParticle.InspectorGraph.Notifications;
using GiantParticle.InspectorGraph.Operations;
using GiantParticle.InspectorGraph.Persistence;
using GiantParticle.InspectorGraph.Plugins;
using GiantParticle.InspectorGraph.UIToolkit;
using GiantParticle.InspectorGraph.Views;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;


namespace GiantParticle.InspectorGraph
{
    internal partial class InspectorGraph : EditorWindow
    {
        private ContentViewRegistry _viewRegistry = new();
        private HashSet<InspectorWindow> _waitForResize = new();
        private InspectorGraphToolbar _toolbar;
        private InspectorGraphFooter _footer;

        private INotificationController _notificationController;
        private IGraphController _graphController;
        private IConnectionDrawer _connectionDrawer;
        private WindowOrganizerFactory _windowOrganizerFactory;
        private IOperation<IObjectNode> _currentOperation;
        private IInspectorGraphPlugin[] _plugins;

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
            Initialize();
        }

        public void OnDisable()
        {
            ClearCurrentContent();
        }

        private void Initialize()
        {
            GlobalApplicationContext.Instantiate();
            IApplicationContext context = GlobalApplicationContext.Instance;
            if (!context.Contains<IInspectorGraphUserPreferences>())
                context.Add<IInspectorGraphUserPreferences>(InspectorGraphUserPreferences.instance);

            if (!context.Contains<IInspectorGraphProjectSettings>())
            {
                context.Add<IInspectorGraphProjectSettings>(InspectorGraphProjectSettings.instance);
            }

            if (!context.Contains<ITypeFilterHandler>())
            {
                var settings = context.Get<IInspectorGraphProjectSettings>();
                context.Add<ITypeFilterHandler>(new TypeFilterHandler(settings.FilterSettings));
            }

            if (!context.Contains<IObjectNodeFactory>())
                context.Add<IObjectNodeFactory>(new ObjectNodeFactory());

            if (!context.Contains<IGraphController>())
            {
                _graphController = new GraphController();
                context.Add<IGraphController>(_graphController);
            }

            if (!context.Contains<IContentViewRegistry>())
                context.Add<IContentViewRegistry>(_viewRegistry);

            // Notification
            if (!context.Contains<INotificationController>())
            {
                _notificationController = new NotificationController();
                context.Add<INotificationController>(_notificationController);
            }
        }

        private void CreateGUI()
        {
            LoadLayout();
            LoadPlugins();

            _toolbar = new InspectorGraphToolbar(new InspectorGraphToolbarConfig()
            {
                CreateCallback = CreateContentTree, ResetCallback = ResetView, UpdateCallback = UpdateView
            });
            _toolbarContainer.Add(_toolbar);

            _footer = new InspectorGraphFooter();
            _footerContainer.Add(_footer);
            _footer.ZoomLevelChanged += OnZoomLevelChanged;

            _notificationController.Container = _notificationContainer;

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

            // Configure View once everything is set
            ProcessPlugins(plugin =>
            {
                if (plugin is IInspectorGraphPluginView viewPlugin)
                    viewPlugin.ConfigureView(rootVisualElement);
            });
        }

        private void LoadLayout()
        {
            var asset = UIToolkitHelper.LocateViewForType(this);
            if (asset == null) return;
            asset.CloneTree(rootVisualElement);
            UIToolkitHelper.ResolveVisualElements(this, rootVisualElement);
        }

        private void LoadPlugins()
        {
            _plugins = ReflectionHelper.InstantiateAllImplementations<IInspectorGraphPlugin>();
            ProcessPlugins(plugin => plugin.Initialize());
        }

        private void ProcessPlugins(Action<IInspectorGraphPlugin> pluginAction)
        {
            if (_plugins == null) return;

            for (int p = 0; p < _plugins.Length; ++p)
            {
                pluginAction.Invoke(_plugins[p]);
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
            _graphController.ClearActiveGraph();
            if (GlobalApplicationContext.IsInstantiated) GlobalApplicationContext.Instance.Remove<IObjectNode>();
            _waitForResize.Clear();
            _notificationController.ClearNotifications();
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
            if (_graphController.ActiveGraph == null) return;
            _content.transform.position = Vector3.zero;
            CreateContentTree(_graphController.ActiveGraph.Object);
        }

        private void CreateContentTree(Object referenceObject)
        {
            _content.transform.position = Vector3.zero;
            ClearCurrentContent();
            UpdateView(referenceObject);
        }

        private void UpdateView()
        {
            if (_graphController.ActiveGraph == null) return;
            // Clear connections
            _viewRegistry.ExecuteOnEachConnection((connection) => { connection.RemoveFromHierarchy(); });
            _viewRegistry.ClearConnections();
            UpdateView(_graphController.ActiveGraph.Object);
        }

        private void UpdateView(Object referenceObject)
        {
            if (referenceObject == null) return;
            if (_currentOperation != null) return;

            _currentOperation = _graphController.ActiveFactory.CreateGraphFromObject(referenceObject);

            PollCurrentOperation();
        }

        private void PollCurrentOperation()
        {
            bool keepPolling = false;
            switch (_currentOperation.State)
            {
                case OperationState.Pending:
                    keepPolling = true;
                    _footer.UpdateProgress("Pending...", float.Epsilon);
                    break;
                case OperationState.Failed:
                    _footer.UpdateProgress("", 0);
                    _notificationController.ShowNotification(
                        notificationType: NotificationType.Error,
                        message: _currentOperation.Message);
                    if (_currentOperation.Result != null) ProcessNewGraph(_currentOperation.Result);
                    _currentOperation = null;
                    break;
                case OperationState.Finished:
                    _footer.UpdateProgress("", 0);
                    ProcessNewGraph(_currentOperation.Result);
                    if (!string.IsNullOrEmpty(_currentOperation.Message))
                        _notificationController.ShowNotification(
                            notificationType: NotificationType.Info,
                            message: _currentOperation.Message);
                    _currentOperation = null;
                    break;
                case OperationState.Started:
                    keepPolling = true;
                    _footer.UpdateProgress("Scanning...", _currentOperation.Progress);
                    break;
            }

            if (keepPolling) rootVisualElement.schedule.Execute(PollCurrentOperation);
        }

        private void ProcessNewGraph(IObjectNode node)
        {
            GlobalApplicationContext.Instance.Remove<IObjectNode>();
            GlobalApplicationContext.Instance.Add<IObjectNode>(_graphController.ActiveGraph);

            Queue<IObjectNode> queue = new();
            HashSet<IObjectNode> visitedNodes = new();
            List<InspectorWindow> newWindows = new();
            queue.Enqueue(_graphController.ActiveGraph);

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
                foreach (IObjectReference nodeReference in item.References)
                    queue.Enqueue(nodeReference.TargetNode);
            }

            DeleteLingeringWindows();

            // Draw Lines
            if (_connectionDrawer == null) _connectionDrawer = new ConnectionDrawer();
            _connectionDrawer.DrawConnections(_content);

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
            queue.Enqueue(_graphController.ActiveGraph);

            while (queue.Count > 0)
            {
                var node = queue.Dequeue();
                if (visitedNodes.Contains(node)) continue;
                visitedNodes.Add(node);

                // Enqueue
                foreach (IObjectReference nodeReference in node.References)
                    queue.Enqueue(nodeReference.TargetNode);
            }

            // Check objects
            HashSet<Object> visitedObjects = new(visitedNodes.Count);
            foreach (IObjectNode visitedNode in visitedNodes)
                visitedObjects.Add(visitedNode.Object);
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
            var projectSettings = GlobalApplicationContext.Instance.Get<IInspectorGraphProjectSettings>();
            var key = node.Object;
            if (_viewRegistry.IsWindowRegisteredByTarget(key))
            {
                // Update Window
                var windowToUpdate = _viewRegistry.WindowByTarget(key);
                windowToUpdate.Node = node;
                windowToUpdate.UpdateView();
                return null;
            }
            if (_viewRegistry.WindowCount > projectSettings.WindowSettings.MaxWindows)
            {
                Debug.LogWarning("Max Window limit reached");
                return null;
            }

            var window = new InspectorWindow(
                node: node,
                forceStaticPreview: _viewRegistry.WindowCount > projectSettings.WindowSettings.MaxPreviewWindows);
            window.style.position = new StyleEnum<Position>(Position.Absolute);

            _content.Add(window);
            _viewRegistry.RegisterWindow(window);
            return window;
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

        private void OnWindowGUIChanged(InspectorWindow window)
        {
            UpdateView();
        }

        private void ReorderWindows()
        {
            if (_windowOrganizerFactory == null) _windowOrganizerFactory = new WindowOrganizerFactory();

            IWindowOrganizer organizer = null;
            var graphDirection = _graphController.ActiveFactory.GraphDirection;
            switch (graphDirection)
            {
                case ReferenceDirection.ReferenceTo:
                    organizer = _windowOrganizerFactory.GetWindowOrganizer(WindowOrganizerType.TopDown);
                    break;
                case ReferenceDirection.ReferenceBy:
                    organizer = _windowOrganizerFactory.GetWindowOrganizer(WindowOrganizerType.BottomUp);
                    break;
                default:
                    throw new NotImplementedException($"Unhandled direction [{graphDirection}]");
            }

            organizer.ReorderWindows();
            _content.ResizeToFit<InspectorWindow>();
            UpdateWindowVisibility();
        }

        private void OnInspectorWindowMoved(VisualElement window)
        {
            if (window is InspectorWindow inspectorWindow)
                inspectorWindow.Node.WindowData.HasBeenManuallyMoved = true;
            _content.ResizeToFit<InspectorWindow>();
        }

        private void OnZoomLevelChanged(float zoomScale)
        {
            // Update Content Scale
            _content.transform.scale = new Vector3(zoomScale, zoomScale, 1);

            // Update Scalables
            _viewRegistry.ExecuteOnEachWindow((window) =>
            {
                for (int i = 0; i < window.Manipulators.Count; ++i)
                {
                    var manipulator = window.Manipulators[i];
                    if (manipulator is IScalableManipulator scalableManipulator)
                        scalableManipulator.Scale = zoomScale;
                }
            });
            UpdateWindowVisibility();
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
