// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using GiantParticle.InspectorGraph.Editor.Data.Nodes;
using UnityEngine;

namespace GiantParticle.InspectorGraph.Editor.Settings
{
    internal interface IReferenceColorSettings
    {
        ReferenceType ReferenceType { get; }
        Color NormalColor { get; }
        Color HighlightedColor { get; }
    }

    [Serializable]
    internal class ReferenceColorSettings : IReferenceColorSettings
    {
        [SerializeField]
        private ReferenceType _referenceType;

        [SerializeField]
        private Color _normalColor;

        [SerializeField]
        private Color _highlightedColor;

        public ReferenceType ReferenceType => _referenceType;
        public Color NormalColor => _normalColor;
        public Color HighlightedColor => _highlightedColor;

        public ReferenceColorSettings()
        {
        }

        public ReferenceColorSettings(ReferenceType referenceType, Color normalColor, Color highlightedColor)
        {
            _referenceType = referenceType;
            _normalColor = normalColor;
            _highlightedColor = highlightedColor;
        }
    }
}
