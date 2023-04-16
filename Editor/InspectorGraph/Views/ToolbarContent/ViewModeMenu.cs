// ********************************
// (C) 2022 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using GiantParticle.InspectorGraph.Editor.ContentView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace GiantParticle.InspectorGraph.Editor.ToolbarContent
{
    internal class ViewModeMenu : ToolbarMenu
    {
        public event Action<ContentViewMode> ViewModeChanged;
        private ContentViewMode _viewMode;

        public ContentViewMode ViewMode
        {
            get => _viewMode;
            set
            {
                if (_viewMode == value) return;
                _viewMode = value;
                ViewModeChanged?.Invoke(_viewMode);
            }
        }

        public ViewModeMenu(Object target) : base()
        {
            var modes = Enum.GetValues(typeof(ContentViewMode));
            for (int i = 0; i < modes.Length; ++i)
            {
                ContentViewMode mode = (ContentViewMode)modes.GetValue(i);
                if (!mode.IsObjectCompatible(target)) continue;

                this.menu.AppendAction(
                    actionName: $"{mode}",
                    action: action => { ViewMode = mode; },
                    actionStatusCallback: CheckMenuAction);
            }

            this.text = "View Mode";
        }

        private DropdownMenuAction.Status CheckMenuAction(DropdownMenuAction action)
        {
            if (action.name == $"{ViewMode}")
                return DropdownMenuAction.Status.Checked;
            return DropdownMenuAction.Status.Normal;
        }
    }
}
