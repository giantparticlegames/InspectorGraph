// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using GiantParticle.InspectorGraph.UIToolkit;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GiantParticle.InspectorGraph
{
    internal partial class InspectorGraphFooter
    {
        [VisualElementField]
        private ToolbarButton _resetZoomButton;

        [VisualElementField]
        private Slider _zoomSlider;

        [VisualElementField]
        private Label _currentModeLabel;

        [VisualElementField]
        private ProgressBar _generalProgressBar;
    }
}
