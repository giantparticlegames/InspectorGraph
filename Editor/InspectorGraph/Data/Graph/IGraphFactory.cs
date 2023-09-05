// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using GiantParticle.InspectorGraph.Data.Nodes;
using GiantParticle.InspectorGraph.Operations;
using Object = UnityEngine.Object;

namespace GiantParticle.InspectorGraph.Data.Graph
{
    internal interface IGraphFactory
    {
        IObjectNode CurrentGraph { get; }
        ReferenceDirection GraphDirection { get; }
        void ClearGraph();
        IOperation<IObjectNode> CreateGraphFromObject(Object rootObject);
    }
}
