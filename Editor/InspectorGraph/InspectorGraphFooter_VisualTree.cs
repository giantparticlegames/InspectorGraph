// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GiantParticle.InspectorGraph
{
    internal partial class InspectorGraphFooter
    {
        private ToolbarButton _resetZoomButton;
        private Slider _zoomSlider;

        private void AssignVisualElements()
        {
            _resetZoomButton = this.Q<ToolbarButton>(nameof(_resetZoomButton));
            _zoomSlider = this.Q<Slider>(nameof(_zoomSlider));
        }
    }
}
