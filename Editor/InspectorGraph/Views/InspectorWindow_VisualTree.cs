// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using GiantParticle.InspectorGraph.UIToolkit;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GiantParticle.InspectorGraph.Views
{
    internal partial class InspectorWindow
    {
        [VisualElementField]
        private Label _titleLabel;

        [VisualElementField]
        private VisualElement _objectIcon;

        [VisualElementField]
        private Toolbar _toolbar;

        [VisualElementField]
        private ObjectField _refField;

        [VisualElementField]
        private VisualElement _windowContent;

        [VisualElementField]
        private ScrollView _content;

        [VisualElementField]
        private Toolbar _footer;
    }
}
