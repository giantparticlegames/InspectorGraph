// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using GiantParticle.InspectorGraph.Data.Nodes;
using Object = UnityEngine.Object;

namespace GiantParticle.InspectorGraph.Data.Graph
{
    internal interface IGraphFactory
    {
        IObjectNode CreateGraphFromObject(Object rootObject);
        void CreateGraphFromObject(Object rootObject, Action<IObjectNode> callback);
    }
}
