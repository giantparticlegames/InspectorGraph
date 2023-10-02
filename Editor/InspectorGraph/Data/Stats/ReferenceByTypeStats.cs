// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System.Collections.Generic;
using GiantParticle.InspectorGraph.Data.Nodes;
using UnityEngine;

namespace GiantParticle.InspectorGraph.Data.Stats
{
    internal class ReferenceByTypeStats
    {
        public ReferenceType ReferenceType { get; }
        public int TotalReferences { get; private set; }
        public int TotalUniqueReference { get; private set; }
        private Dictionary<Object, int> _referencesByNode = new();

        public ReferenceByTypeStats(ReferenceType type)
        {
            ReferenceType = type;
        }

        public void AddReference(Object obj)
        {
            ++TotalReferences;
            if (!_referencesByNode.ContainsKey(obj))
            {
                _referencesByNode.Add(obj, 0);
                ++TotalUniqueReference;
            }

            _referencesByNode[obj] += 1;
        }
    }
}
