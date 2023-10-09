// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using GiantParticle.InspectorGraph.UIToolkit;
using UnityEngine.UIElements;

namespace GiantParticle.InspectorGraph.Notifications
{
    internal partial class InspectorGraphNotification
    {
        [VisualElementField]
        private VisualElement _container;

        [VisualElementField]
        private VisualElement _icon;

        [VisualElementField]
        private Label _text;

        [VisualElementField]
        private Button _closeButton;
    }
}
