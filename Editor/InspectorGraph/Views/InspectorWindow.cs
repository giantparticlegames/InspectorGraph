// ********************************
// (C) 2022 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using System.Collections.Generic;
using GiantParticle.InspectorGraph.ContentView;
using GiantParticle.InspectorGraph.Manipulators;
using GiantParticle.InspectorGraph.Data.Nodes;
using GiantParticle.InspectorGraph.Persistence;
using GiantParticle.InspectorGraph.ToolbarContent;
using GiantParticle.InspectorGraph.UIToolkit;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GiantParticle.InspectorGraph.Views
{
    internal partial class InspectorWindow : VisualElement, IDisposable
    {
        private class NoTypeClass
        {
        }

        private ContentViewMode _currentMode;
        private WindowStateController _stateController;
        private List<IManipulator> _manipulators = new();
        private QuickStatsView _quickStatsView;
        private BaseWindowContent _view;
        private bool _forceStaticPreviewMode;

        public IObjectNode Node { get; set; }
        public IReadOnlyList<IManipulator> Manipulators => _manipulators;
        public event Action<InspectorWindow> GUIChanged;


        public InspectorWindow(IObjectNode node, bool forceStaticPreview = false)
        {
            Node = node;
            _forceStaticPreviewMode = forceStaticPreview;
            CreateLayout();
            ConfigureWindowManipulation();
            UpdateSettings();
        }

        public void Dispose()
        {
            DisposeContent();
            _stateController?.Dispose();
        }

        private void CreateLayout()
        {
            var asset = UIToolkitHelper.LocateViewForType(this);
            if (asset == null) return;
            asset.CloneTree(this);
            UIToolkitHelper.ResolveVisualElements(this, this);

            UpdateTitle();

            ContentViewMode preferredMode = _forceStaticPreviewMode
                ? ContentViewMode.StaticPreview
                : WindowContentFactory.PreferredViewModeForObject(Node.Object);
            ConfigureToolbar(preferredMode);

            // Footer
            _quickStatsView = new QuickStatsView(Node);
            _footer.Add(_quickStatsView);

            SwitchView(preferredMode);
        }

        private void UpdateTitle()
        {
            _titleLabel.text = $"[{Node.Object.name}]";
            _objectIcon.style.backgroundImage = new StyleBackground(AssetPreview.GetMiniThumbnail(Node.Object));
        }

        private void ConfigureToolbar(ContentViewMode preferredMode)
        {
            var viewModeMenu = new ViewModeMenu(Node.Object);
            viewModeMenu.ViewMode = preferredMode;
            viewModeMenu.ViewModeChanged += SwitchView;
            _toolbar.Add(viewModeMenu);

            _refField.value = Node.Object;
            _refField.objectType = typeof(NoTypeClass);
        }

        private void ConfigureWindowManipulation()
        {
            var header = this.Q<VisualElement>("_header");
            _stateController = new WindowStateController(window: this, windowContent: _windowContent);
            // Minimize
            _stateController.RegisterCase("_minimizeButton", WindowStateController.State.Minimized);
            _stateController.RegisterCase("_maximizeButton", WindowStateController.State.Maximized);

            // Move
            var dragManipulator = new DragManipulator(header, this);
            _manipulators.Add(dragManipulator);
            // Resize
            var resizeButton = this.Q<VisualElement>("_resizeCorner");
            var resizeManipulator = new ResizeManipulator(
                handle: resizeButton,
                resizeTarget: this,
                minValues: new Vector2(280, 60));
            resizeManipulator.TargetResized += _stateController.ForceNormalState;
            _manipulators.Add(resizeManipulator);

            // Highlight
            var highlight = new HighlightManipulator(handle: header,
                owner: this,
                mode: HighlightManipulator.Mode.All);
            _manipulators.Add(highlight);
            var highlightLock = this.Q<VisualElement>("_highlight");
            highlightLock.RegisterCallback<ClickEvent>(evt =>
            {
                highlight.ForceActive = !highlight.ForceActive;
                if (highlight.ForceActive) highlightLock.AddToClassList("active");
                else highlightLock.RemoveFromClassList("active");
            });

            // Front
            var bringToFront = new BringToFrontManipulator(this);
            _manipulators.Add(bringToFront);
        }

        private void UpdateSettings()
        {
            var projectSettings = GlobalApplicationContext.Instance.Get<IInspectorGraphProjectSettings>();
            var sizeSettings = projectSettings.WindowSettings.GetWindowSizeSettings(_currentMode);
            this.style.width = new StyleLength(sizeSettings.Size.x);
            this.style.maxHeight = new StyleLength(sizeSettings.Size.y);
            if (_currentMode == ContentViewMode.Preview || _currentMode == ContentViewMode.StaticPreview)
                this.style.height = new StyleLength(sizeSettings.Size.y);
        }

        public void UpdateView()
        {
            _quickStatsView.Node = Node;
            _quickStatsView.UpdateView();
        }

        public void DisposeContent()
        {
            if (_view == null) return;
            _view.ContentChanged -= OnContentChanged;
            _view.RemoveFromHierarchy();
            _view.Dispose();
            _view = null;
        }

        private void SwitchView(ContentViewMode mode)
        {
            DisposeContent();
            _content.contentContainer.RemoveFromClassList($"{_currentMode}");

            _currentMode = mode;
            _view = WindowContentFactory.CreateContent(_currentMode, Node.WindowData, _forceStaticPreviewMode);
            _view.ContentChanged += OnContentChanged;
            _content.Add(_view);
            _content.contentContainer.AddToClassList($"{mode}");
        }

        private void OnContentChanged()
        {
            GUIChanged?.Invoke(this);
        }
    }
}
