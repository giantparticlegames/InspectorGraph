// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using GiantParticle.InspectorGraph.UIToolkit;
using UnityEngine.UIElements;

namespace GiantParticle.InspectorGraph
{
    internal partial class InspectorGraph
    {
        [VisualElementField]
        private VisualElement _windowView;

        [VisualElementField]
        private VisualElement _content;

        [VisualElementField]
        private VisualElement _toolbarContainer;

        [VisualElementField]
        private VisualElement _notificationContainer;

        [VisualElementField]
        private VisualElement _footerContainer;
    }
}
