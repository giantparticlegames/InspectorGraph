// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using GiantParticle.InspectorGraph.UIToolkit;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GiantParticle.InspectorGraph
{
    internal partial class InspectorGraphToolbar
    {
        [VisualElementField]
        private ToolbarMenu _viewMenu;

        [VisualElementField]
        private ToolbarMenu _editMenu;

        [VisualElementField]
        private ToolbarMenu _helpMenu;

        [VisualElementField]
        private VisualElement _activeObjectContainer;

        [VisualElementField]
        private ObjectField _refField;
    }
}
