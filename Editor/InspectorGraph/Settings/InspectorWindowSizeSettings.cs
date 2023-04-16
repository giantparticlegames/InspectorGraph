// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using GiantParticle.InspectorGraph.Editor.ContentView;
using UnityEngine;

namespace GiantParticle.InspectorGraph.Editor.Settings
{
    internal interface IInspectorWindowSizeSettings
    {
        ContentViewMode Mode { get; }
        Vector2Int Size { get; }
    }

    [Serializable]
    internal class InspectorWindowSizeSettings : IInspectorWindowSizeSettings
    {
        [SerializeField]
        internal ContentViewMode _targetMode;

        [SerializeField]
        internal Vector2Int _targetSize;

        public ContentViewMode Mode => _targetMode;
        public Vector2Int Size => _targetSize;

        public InspectorWindowSizeSettings()
        {
        }

        public InspectorWindowSizeSettings(ContentViewMode mode, Vector2Int size)
        {
            _targetMode = mode;
            _targetSize = size;
        }
    }
}
