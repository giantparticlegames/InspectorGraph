// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System.Collections.Generic;
using GiantParticle.InspectorGraph.Data.Nodes;
using UnityEngine;

namespace GiantParticle.InspectorGraph.Data.Stats
{
    internal class ReferenceStats
    {
        public int TotalReferences { get; private set; }
        public int UniqueReferences => _uniqueReferences.Count;

        private Dictionary<ReferenceType, ReferenceByTypeStats> _referenceByType = new();
        private HashSet<Object> _uniqueReferences = new();

        public int ReferenceTypes => _referenceByType.Count;

        public IEnumerable<ReferenceByTypeStats> StatsByType
        {
            get
            {
                foreach (ReferenceByTypeStats referenceByTypeStats in _referenceByType.Values)
                {
                    yield return referenceByTypeStats;
                }
            }
        }

        public void AddToStats(IObjectReference reference)
        {
            if (!_referenceByType.ContainsKey(reference.RefType))
                _referenceByType.Add(reference.RefType, new ReferenceByTypeStats(reference.RefType));

            var obj = reference.TargetNode.Object;
            _uniqueReferences.Add(obj);
            _referenceByType[reference.RefType].AddReference(obj);
            ++TotalReferences;
        }
    }
}
