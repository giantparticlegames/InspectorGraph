// ********************************
// (C) 2022 - Giant Particle Games
// All rights reserved.
// ********************************

namespace GiantParticle.InspectorGraph.Data.Nodes
{
    internal class ObjectReference : IObjectReference
    {
        public ReferenceDirection Direction { get; }
        public ReferenceType RefType { get; }
        public IObjectNode TargetNode { get; }

        public ObjectReference(IObjectNode targetNode, ReferenceType referenceType, ReferenceDirection direction)
        {
            Direction = direction;
            RefType = referenceType;
            TargetNode = targetNode;
        }
    }
}
