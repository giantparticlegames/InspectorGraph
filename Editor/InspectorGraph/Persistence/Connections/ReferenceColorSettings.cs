// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using GiantParticle.InspectorGraph.Data.Nodes;
using UnityEngine;

namespace GiantParticle.InspectorGraph.Persistence
{
    [Serializable]
    internal class ReferenceColorSettings : IReferenceColorSettings
    {
        [SerializeField]
        internal ReferenceType _referenceType;

        [SerializeField]
        internal Color _normalColor;

        [SerializeField]
        internal Color _highlightedColor;

        public ReferenceType ReferenceType => _referenceType;
        public Color NormalColor => _normalColor;
        public Color HighlightedColor => _highlightedColor;

        public ReferenceColorSettings(ReferenceType referenceType, Color normalColor, Color highlightedColor)
        {
            _referenceType = referenceType;
            _normalColor = normalColor;
            _highlightedColor = highlightedColor;
        }
    }
}
