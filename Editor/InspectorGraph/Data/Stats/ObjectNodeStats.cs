// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using GiantParticle.InspectorGraph.Data.Nodes;

namespace GiantParticle.InspectorGraph.Data.Stats
{
    internal static class ObjectNodeStats
    {
        public static ReferenceStats GetReferenceStats(IObjectNode node, ReferenceDirection direction)
        {
            ReferenceStats stats = new ReferenceStats();
            for (int i = 0; i < node.References.Count; ++i)
            {
                var reference = node.References[i];
                if (reference.Direction != direction) continue;
                stats.AddToStats(reference);
            }

            return stats;
        }
    }
}
