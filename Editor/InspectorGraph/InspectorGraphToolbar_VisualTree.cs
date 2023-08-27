// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GiantParticle.InspectorGraph
{
    internal partial class InspectorGraphToolbar
    {
        private ToolbarMenu _viewMenu;
        private ToolbarMenu _editMenu;
        private ToolbarMenu _helpMenu;
        private VisualElement _activeObjectContainer;
        private DropdownField _inspectionModeDropdown;
        private ObjectField _refField;

        private void AssignVisualElements()
        {
            _viewMenu = this.Q<ToolbarMenu>(nameof(_viewMenu));
            _editMenu = this.Q<ToolbarMenu>(nameof(_editMenu));
            _helpMenu = this.Q<ToolbarMenu>(nameof(_helpMenu));
            _activeObjectContainer = this.Q<VisualElement>(nameof(_activeObjectContainer));
            _refField = this.Q<ObjectField>(nameof(_refField));
            _inspectionModeDropdown = this.Q<DropdownField>(nameof(_inspectionModeDropdown));
        }
    }
}
