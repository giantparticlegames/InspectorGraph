// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

namespace GiantParticle.InspectorGraph.Editor.Data.Nodes
{
    public enum ReferenceType
    {
        Direct = 0,
        NestedPrefab = 1,
#if INSPECTOR_GRAPH_PRO
        Addressable = 2
#endif
    }
}
