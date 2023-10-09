// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using System.Collections.Generic;
using GiantParticle.InspectorGraph.Data.Nodes;
using UnityEngine;

namespace GiantParticle.InspectorGraph.Persistence
{
    [Serializable]
    internal class ConnectionSettings : IConnectionSettings
    {
        [SerializeField]
        internal bool _drawReferenceCount = true;

        [SerializeField]
        internal List<ReferenceColorSettings> _colorSettings = new List<ReferenceColorSettings>()
        {
            new ReferenceColorSettings(
                referenceType: ReferenceType.Direct,
                normalColor: new Color(1, 1, 1, 0.5f),
                highlightedColor: new Color(1, 1, 1, 1)),
            new ReferenceColorSettings(
                referenceType: ReferenceType.NestedPrefab,
                normalColor: new Color(0, 1, 1, 0.5f),
                highlightedColor: new Color(0, 1, 1, 1))
        };

        public bool DrawReferenceCount => _drawReferenceCount;
        public List<ReferenceColorSettings> ColorSettings => _colorSettings;

        public ReferenceColorSettings GetColorSettings(ReferenceType referenceType)
        {
            for (int i = 0; i < _colorSettings.Count; ++i)
            {
                if (_colorSettings[i].ReferenceType == referenceType)
                    return _colorSettings[i];
            }

            return null;
        }
    }
}
