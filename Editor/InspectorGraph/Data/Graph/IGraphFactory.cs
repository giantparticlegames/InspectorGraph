// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using GiantParticle.InspectorGraph.Data.Nodes;
using GiantParticle.InspectorGraph.Operations;
using Object = UnityEngine.Object;

namespace GiantParticle.InspectorGraph.Data.Graph
{
    internal interface IGraphFactory
    {
        IOperation<IObjectNode> CreateGraphFromObject(Object rootObject);
    }
}
