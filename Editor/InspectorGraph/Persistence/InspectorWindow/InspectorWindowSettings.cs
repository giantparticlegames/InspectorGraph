// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using System.Collections.Generic;
using GiantParticle.InspectorGraph.ContentView;
using UnityEngine;

namespace GiantParticle.InspectorGraph.Persistence
{
    [Serializable]
    internal class InspectorWindowSettings : IInspectorWindowSettings
    {
        [SerializeField]
        internal int _maxWindows = 100;

        [SerializeField]
        internal int _maxPreviewWindows = 50;

        [SerializeField]
        internal List<InspectorWindowSizeSettings> _windowSizeSettings = new List<InspectorWindowSizeSettings>()
        {
            new InspectorWindowSizeSettings(
                mode: ContentViewMode.Preview,
                size: new Vector2Int(300, 300)),
            new InspectorWindowSizeSettings(
                mode: ContentViewMode.InspectorElement,
                size: new Vector2Int(300, 300)),
            new InspectorWindowSizeSettings(
                mode: ContentViewMode.StaticPreview,
                size: new Vector2Int(300, 150)),
            new InspectorWindowSizeSettings(
                mode: ContentViewMode.IMGUI,
                size: new Vector2Int(400, 300))
        };

        public int MaxPreviewWindows
        {
            get => _maxPreviewWindows;
            set => _maxPreviewWindows = value;
        }

        public int MaxWindows
        {
            get => _maxWindows;
            set => _maxWindows = value;
        }

        public List<InspectorWindowSizeSettings> WindowSizeSettings => _windowSizeSettings;

        public InspectorWindowSizeSettings GetWindowSizeSettings(ContentViewMode mode)
        {
            if (_windowSizeSettings == null) return null;
            for (int i = 0; i < _windowSizeSettings.Count; ++i)
            {
                var settings = _windowSizeSettings[i];
                if (settings.Mode == mode) return settings;
            }

            return null;
        }
    }
}
