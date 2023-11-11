// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System.Collections.Generic;
using UnityEngine;

namespace GiantParticle.InspectorGraph.Data.Nodes
{
    internal interface IObjectNode
    {
        Object Object { get; }
        IWindowData WindowData { get; }
        IReadOnlyList<IObjectReference> References { get; }
    }
}
