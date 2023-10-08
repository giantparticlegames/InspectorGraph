// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System.Collections.Generic;
using GiantParticle.InspectorGraph.ContentView;

namespace GiantParticle.InspectorGraph.Persistence
{
    internal interface IInspectorWindowSettings
    {
        int MaxPreviewWindows { get; set; }
        int MaxWindows { get; set; }
        List<InspectorWindowSizeSettings> WindowSizeSettings { get; }
        InspectorWindowSizeSettings GetWindowSizeSettings(ContentViewMode mode);
    }
}
