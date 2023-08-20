// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using System.Collections.Generic;
using GiantParticle.InspectorGraph.Data.Graph.SubTree.SerializedPropertyProcessors;
using GiantParticle.InspectorGraph.Data.Nodes;

namespace GiantParticle.InspectorGraph.Data.Graph.SubTree.ObjectNodeProcessors
{
    internal interface IObjectNodeProcessor
    {
        Type TargetType { get; }
        void SetPropertyProcessors(IReadOnlyList<ISerializedPropertyProcessor> processors);
        void ProcessNode(ObjectNode node);
    }
}
