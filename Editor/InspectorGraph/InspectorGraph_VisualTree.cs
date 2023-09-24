// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using UnityEngine.UIElements;

namespace GiantParticle.InspectorGraph
{
    internal partial class InspectorGraph
    {
        private VisualElement _windowView;
        private VisualElement _content;
        private VisualElement _toolbarContainer;
        private VisualElement _notificationContainer;
        private VisualElement _footerContainer;

        private void AssignVisualElements()
        {
            _windowView = rootVisualElement.Q<VisualElement>(nameof(_windowView));
            _content = rootVisualElement.Q<VisualElement>(nameof(_content));
            _toolbarContainer = rootVisualElement.Q<VisualElement>(nameof(_toolbarContainer));
            _footerContainer = rootVisualElement.Q<VisualElement>(nameof(_footerContainer));
            _notificationContainer = rootVisualElement.Q<VisualElement>(nameof(_notificationContainer));
        }
    }
}
