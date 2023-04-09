// ********************************
// (C) 2022 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using System.Collections.Generic;
using GiantParticle.InspectorGraph.Editor.ContentView;
using GiantParticle.InspectorGraph.Editor.Manipulators;
using GiantParticle.InspectorGraph.Editor.Data.Nodes;
using GiantParticle.InspectorGraph.Editor.Settings;
using GiantParticle.InspectorGraph.Editor.ToolbarContent;
using GiantParticle.InspectorGraph.Editor.UIDocuments;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GiantParticle.InspectorGraph.Editor.Views
{
    internal class InspectorWindow : VisualElement, IDisposable
    {
        private class NoTypeClass
        {
        }

        private VisualElement _windowContent;
        private ScrollView _content;
        private Toolbar _toolbar;
        private Toolbar _footer;
        private ContentViewMode _currentMode;
        private WindowStateController _stateController;
        private List<IManipulator> _manipulators = new();
        private QuickStatsView _quickStatsView;
        private BaseWindowContent _view;

        public IReadOnlyList<IManipulator> Manipulators => _manipulators;
        public event Action GUIChanged;

        public IObjectNode Node { get; }
        private bool _forceStaticPreviewMode;


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
            var catalog = GlobalApplicationContext.Instance.Get<IUIDocumentCatalog<InspectorWindowUIDocumentType>>();
            var xmlLayout = catalog[InspectorWindowUIDocumentType.InspectorWindow].Asset;
            xmlLayout.CloneTree(this);

            _content = this.Q<ScrollView>(nameof(_content));
            _windowContent = this.Q<VisualElement>(nameof(_windowContent));
            _footer = this.Q<Toolbar>(nameof(_footer));
            _toolbar = this.Q<Toolbar>(nameof(_toolbar));

            var title = this.Q<Label>("_titleLabel");
            title.text = $"[{Node.Target.name}]";

            // Toolbar
            ContentViewMode preferredMode = _forceStaticPreviewMode
                ? ContentViewMode.StaticPreview
                : WindowContentFactory.PreferredViewModeForObject(Node.Target);
            var viewModeMenu = new ViewModeMenu(Node.Target);
            viewModeMenu.ViewMode = preferredMode;
            viewModeMenu.ViewModeChanged += SwitchView;
            _toolbar.Add(viewModeMenu);

            var refField = this.Q<ObjectField>("_refField");
            refField.value = Node.Target;
            refField.objectType = typeof(NoTypeClass);

            // Footer
            _quickStatsView = new QuickStatsView(Node);
            _footer.Add(_quickStatsView);

            SwitchView(preferredMode);
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
            var resizeManipulator = new ResizeManipulator(resizeButton, this);
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
            var settings = GlobalApplicationContext.Instance.Get<IInspectorGraphSettings>();
            var sizeSettings = settings.GetSizeForWindowViewMode(_currentMode);
            this.style.width = new StyleLength(sizeSettings.Size.x);
            this.style.maxHeight = new StyleLength(sizeSettings.Size.y);
            if (_currentMode == ContentViewMode.Preview || _currentMode == ContentViewMode.StaticPreview)
                this.style.height = new StyleLength(sizeSettings.Size.y);
        }

        public void UpdateView()
        {
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
            GUIChanged?.Invoke();
        }
    }
}
