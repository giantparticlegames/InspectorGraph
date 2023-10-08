// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GiantParticle.InspectorGraph.Persistence
{
    [Serializable]
    internal class FilterSettings : IFilterSettings
    {
        [SerializeField]
        internal bool _enableFilters = true;

        [SerializeField]
        internal List<FilterTypeSetting> _typeFilters = new List<FilterTypeSetting>()
        {
            new FilterTypeSetting(
                fullyQualifiedName: typeof(GameObject).FullName,
                showType: true,
                expandType: false),
            new FilterTypeSetting(
                fullyQualifiedName: typeof(MonoScript).FullName,
                showType: false,
                expandType: false
            ),
            new FilterTypeSetting(
                fullyQualifiedName: "UnityEngine.U2D.SpriteAtlas",
                showType: true,
                expandType: false
            )
        };

        public bool EnableFilters => _enableFilters;
        public List<FilterTypeSetting> TypeFilters => _typeFilters;
    }
}
