// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

namespace GiantParticle.InspectorGraph.Editor.Data.Nodes
{
    public enum ReferenceType
    {
        Direct,
        HierarchyEmbedded,
#if INSPECTOR_GRAPH_PRO
        Addressable
#endif
    }
}
