// ********************************
// (C) 2022 - Giant Particle Games
// All rights reserved.
// ********************************

namespace GiantParticle.InspectorGraph.Editor.Data.Nodes
{
    internal interface IObjectNodeReference
    {
        ReferenceType RefType { get; }
        IObjectNode TargetNode { get; }
        int RefCount { get; }
    }

    internal class ObjectNodeReference : IObjectNodeReference
    {
        public ReferenceType RefType { get; }
        public IObjectNode TargetNode { get; }
        public int RefCount { get; set; }

        public ObjectNodeReference(IObjectNode targetNode, ReferenceType refType)
        {
            TargetNode = targetNode;
            RefCount = 1;
            RefType = refType;
        }
    }
}
