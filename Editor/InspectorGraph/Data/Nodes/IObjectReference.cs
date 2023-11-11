// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

namespace GiantParticle.InspectorGraph.Data.Nodes
{
    internal interface IObjectReference
    {
        ReferenceDirection Direction { get; }
        ReferenceType RefType { get; }
        IObjectNode TargetNode { get; }
    }
}
