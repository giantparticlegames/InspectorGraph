// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using System.Collections.Generic;
using GiantParticle.InspectorGraph.Editor.Data.Nodes.SerializedPropertyProcessors;

namespace GiantParticle.InspectorGraph.Editor.Data.Nodes.ObjectNodeProcessors
{
    internal interface IObjectNodeProcessor
    {
        Type TargetType { get; }
        void SetPropertyProcessors(IReadOnlyList<ISerializedPropertyProcessor> processors);
        void ProcessNode(ObjectNode node);
    }
}
