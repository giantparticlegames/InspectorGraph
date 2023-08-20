// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using GiantParticle.InspectorGraph.Data.Nodes;
using UnityEngine;

namespace GiantParticle.InspectorGraph.Data.Graph
{
    internal interface IGraphFactory
    {
        // TODO: Move this to an Attribute
        int DisplayPriority { get; }
        string DisplayName { get; }
        IObjectNode CreateGraphFromObject(Object rootObject);
    }
}
