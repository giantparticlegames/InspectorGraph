// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using GiantParticle.InspectorGraph.Data.Nodes;
using UnityEngine;

namespace GiantParticle.InspectorGraph.Persistence
{
    internal interface IReferenceColorSettings
    {
        ReferenceType ReferenceType { get; }
        Color NormalColor { get; }
        Color HighlightedColor { get; }
    }
}
