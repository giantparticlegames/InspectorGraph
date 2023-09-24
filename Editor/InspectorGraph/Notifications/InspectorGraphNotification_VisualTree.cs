// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using UnityEngine.UIElements;

namespace GiantParticle.InspectorGraph.Notifications
{
    internal partial class InspectorGraphNotification
    {
        private VisualElement _container;
        private VisualElement _icon;
        private Label _text;
        private Button _closeButton;

        private void AssignVisualElements()
        {
            _container = this.Q<VisualElement>(nameof(_container));
            _icon = this.Q<VisualElement>(nameof(_icon));
            _text = this.Q<Label>(nameof(_text));
            _closeButton = this.Q<Button>(nameof(_closeButton));
        }
    }
}
