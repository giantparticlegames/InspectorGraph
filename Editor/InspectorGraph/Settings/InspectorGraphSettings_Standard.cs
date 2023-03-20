#if !INSPECTOR_GRAPH_PRO
// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using System.Collections.Generic;
using GiantParticle.InspectorGraph.Editor.Data.Nodes;
using UnityEngine;

namespace GiantParticle.InspectorGraph.Settings
{
    internal partial class InspectorGraphSettings
    {
        private partial void EnsureDefaultReferenceColors()
        {
            if (_referenceColors == null) _referenceColors = new List<ReferenceColorSettings>();
            var values = Enum.GetValues(typeof(ReferenceType));
            if (_referenceColors.Count != values.Length)
            {
                _referenceColors.Clear();
                _referenceColors.Add(new ReferenceColorSettings(
                    referenceType: ReferenceType.Direct,
                    normalColor: new Color(1, 1, 1, 0.5f),
                    highlightedColor: new Color(1, 1, 1, 1)));
                _referenceColors.Add(new ReferenceColorSettings(
                    referenceType: ReferenceType.NestedPrefab,
                    normalColor: new Color(0, 1, 1, 0.5f),
                    highlightedColor: new Color(0, 1, 1, 1)));
            }
        }
    }
}
#endif
