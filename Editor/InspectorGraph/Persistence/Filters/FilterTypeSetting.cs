// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using UnityEngine;

namespace GiantParticle.InspectorGraph.Persistence
{
    [Serializable]
    internal class FilterTypeSetting : IFilterTypeSetting
    {
        [SerializeField]
        internal string _fullyQualifiedName;

        [SerializeField]
        internal bool _showType;

        [SerializeField]
        internal bool _expandType;

        public string FullyQualifiedName => _fullyQualifiedName;
        public bool ShowType => _showType;
        public bool ExpandType => _expandType;

        public FilterTypeSetting(string fullyQualifiedName, bool showType, bool expandType)
        {
            _fullyQualifiedName = fullyQualifiedName;
            _showType = showType;
            _expandType = expandType;
        }
    }
}
