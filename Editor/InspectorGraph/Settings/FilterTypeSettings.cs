// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace GiantParticle.InspectorGraph.Editor.Settings
{
    internal interface IFilterTypeSettings
    {
        string FullyQualifiedName { get; }
        bool ShowType { get; }
        bool ExpandType { get; }
    }

    [Serializable]
    internal class FilterTypeSettings : IFilterTypeSettings
    {
        [SerializeField]
        [FormerlySerializedAs("FullyQualifiedName")]
        private string _fullyQualifiedName;

        [SerializeField]
        [FormerlySerializedAs("ShowType")]
        private bool _showType;

        [SerializeField]
        [FormerlySerializedAs("ExpandType")]
        private bool _expandType;

        public string FullyQualifiedName => _fullyQualifiedName;
        public bool ShowType => _showType;
        public bool ExpandType => _expandType;

        public FilterTypeSettings()
        {
        }

        public FilterTypeSettings(string fullyQualifiedName, bool showType, bool expandType)
        {
            _fullyQualifiedName = fullyQualifiedName;
            _showType = showType;
            _expandType = expandType;
        }
    }
}
