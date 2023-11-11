// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using GiantParticle.InspectorGraph.ContentView;
using UnityEngine;

namespace GiantParticle.InspectorGraph.Persistence
{
    internal interface IInspectorWindowSizeSettings
    {
        ContentViewMode Mode { get; }
        Vector2Int Size { get; }
    }
}
