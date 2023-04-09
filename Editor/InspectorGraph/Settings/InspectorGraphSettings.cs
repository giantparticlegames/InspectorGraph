// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using System.Collections.Generic;
using System.IO;
using GiantParticle.InspectorGraph.Editor.ContentView;
using GiantParticle.InspectorGraph.Editor.Data.Nodes;
using UnityEditor;
using UnityEngine;

namespace GiantParticle.InspectorGraph.Editor.Settings
{
    internal interface IInspectorGraphSettings
    {
        int MaxPreviewWindows { get; }
        int MaxWindows { get; }
        IReadOnlyList<IFilterTypeSettings> TypeFilters { get; }
        IReadOnlyList<IReferenceColorSettings> ReferenceColorSettings { get; }
        IReadOnlyList<IInspectorWindowSizeSettings> InspectorWindowSizeSettings { get; }

        IReferenceColorSettings GetReferenceColor(ReferenceType type);
        IInspectorWindowSizeSettings GetSizeForWindowViewMode(ContentViewMode mode);
    }

    internal interface IExtendedInspectorGraphSettings
    {
        void AddExtendedReferenceColors(List<ReferenceColorSettings> referenceColors);
    }

    internal class InspectorGraphSettings : ScriptableObject, IInspectorGraphSettings
    {
        public const string kDefaultSettingsLocation =
            "Assets/Editor/com.giantparticle.inspector_graph/InspectorGraphSettings.asset";

        [Header("Visualization Settings")]
        [SerializeField]
        internal int _maxPreviewWindows = 50;

        [SerializeField]
        internal int _maxWindows = 100;

        [SerializeField]
        internal List<ReferenceColorSettings> _referenceColors;

        [Header("Inspector Window Settings")]
        [SerializeField]
        internal List<FilterTypeSettings> _filters;

        [SerializeField]
        internal List<InspectorWindowSizeSettings> _defaultWindowSize;

        public int MaxPreviewWindows => _maxPreviewWindows;
        public int MaxWindows => _maxWindows;
        public IReadOnlyList<IFilterTypeSettings> TypeFilters => _filters;
        public IReadOnlyList<IReferenceColorSettings> ReferenceColorSettings => _referenceColors;
        public IReadOnlyList<IInspectorWindowSizeSettings> InspectorWindowSizeSettings => _defaultWindowSize;

        [NonSerialized]
        private Dictionary<ReferenceType, IReferenceColorSettings> _referenceColorMap;

        [NonSerialized]
        private Dictionary<ContentViewMode, IInspectorWindowSizeSettings> _windowSizeMap;

        public IReferenceColorSettings GetReferenceColor(ReferenceType type)
        {
            if (_referenceColorMap == null)
            {
                _referenceColorMap = new Dictionary<ReferenceType, IReferenceColorSettings>();
                for (int i = 0; i < _referenceColors.Count; ++i)
                {
                    ReferenceColorSettings referenceColor = _referenceColors[i];
                    _referenceColorMap.Add(referenceColor.ReferenceType, referenceColor);
                }
            }

            return _referenceColorMap[type];
        }

        public IInspectorWindowSizeSettings GetSizeForWindowViewMode(ContentViewMode mode)
        {
            if (_windowSizeMap == null)
            {
                _windowSizeMap = new Dictionary<ContentViewMode, IInspectorWindowSizeSettings>();
                for (int i = 0; i < _defaultWindowSize.Count; ++i)
                {
                    InspectorWindowSizeSettings windowSize = _defaultWindowSize[i];
                    _windowSizeMap.Add(windowSize.Mode, windowSize);
                }
            }

            return _windowSizeMap[mode];
        }

        private void OnEnable()
        {
            EnsureDefaultFilters();
            EnsureDefaultReferenceColors();
            EnsureDefaultWindowSizes();
        }

        private void EnsureDefaultReferenceColors()
        {
            if (_referenceColors == null) _referenceColors = new List<ReferenceColorSettings>();

            if (!_referenceColors.Exists(settings => settings.ReferenceType == ReferenceType.Direct))
            {
                _referenceColors.Add(new ReferenceColorSettings(
                    referenceType: ReferenceType.Direct,
                    normalColor: new Color(1, 1, 1, 0.5f),
                    highlightedColor: new Color(1, 1, 1, 1)));
            }

            if (!_referenceColors.Exists(settings => settings.ReferenceType == ReferenceType.NestedPrefab))
            {
                _referenceColors.Add(new ReferenceColorSettings(
                    referenceType: ReferenceType.NestedPrefab,
                    normalColor: new Color(0, 1, 1, 0.5f),
                    highlightedColor: new Color(0, 1, 1, 1)));
            }

            var extendedInstances = ReflectionHelper.InstantiateAllImplementations<IExtendedInspectorGraphSettings>();
            for (int i = 0; i < extendedInstances.Length; ++i)
                extendedInstances[i].AddExtendedReferenceColors(_referenceColors);
        }

        private void EnsureDefaultWindowSizes()
        {
            if (_defaultWindowSize == null) _defaultWindowSize = new List<InspectorWindowSizeSettings>();

            if (!_defaultWindowSize.Exists(settings => settings.Mode == ContentViewMode.Preview))
            {
                _defaultWindowSize.Add(new InspectorWindowSizeSettings(mode: ContentViewMode.Preview,
                    size: new Vector2Int(300, 300)));
            }
            if (!_defaultWindowSize.Exists(settings => settings.Mode == ContentViewMode.InspectorElement))
            {
                _defaultWindowSize.Add(new InspectorWindowSizeSettings(mode: ContentViewMode.InspectorElement,
                    size: new Vector2Int(300, 300)));
            }
            if (!_defaultWindowSize.Exists(settings => settings.Mode == ContentViewMode.StaticPreview))
            {
                _defaultWindowSize.Add(new InspectorWindowSizeSettings(mode: ContentViewMode.StaticPreview,
                    size: new Vector2Int(300, 130)));
            }
            if (!_defaultWindowSize.Exists(settings => settings.Mode == ContentViewMode.IMGUI))
            {
                _defaultWindowSize.Add(new InspectorWindowSizeSettings(mode: ContentViewMode.IMGUI,
                    size: new Vector2Int(400, 300)));
            }
        }

        private void EnsureDefaultFilters()
        {
            if (_filters == null) _filters = new List<FilterTypeSettings>();
            if (_filters.Count <= 0)
            {
                _filters.Add(new FilterTypeSettings(
                    fullyQualifiedName: typeof(GameObject).FullName,
                    showType: true,
                    expandType: false));
                _filters.Add(new FilterTypeSettings(
                    fullyQualifiedName: typeof(MonoScript).FullName,
                    showType: false,
                    expandType: false
                ));
                _filters.Add(new FilterTypeSettings(
                    fullyQualifiedName: "UnityEngine.U2D.SpriteAtlas",
                    showType: true,
                    expandType: false
                ));
            }
        }

        internal static InspectorGraphSettings GetSettings()
        {
            // Check if the settings are in the default location
            var settings = AssetDatabase.LoadAssetAtPath<InspectorGraphSettings>(kDefaultSettingsLocation);
            if (settings != null) return settings;
            // Check if the settings are somewhere else
            string[] guids = AssetDatabase.FindAssets($"t:{typeof(InspectorGraphSettings).FullName}");
            if (guids != null && guids.Length > 0)
                return AssetDatabase.LoadAssetAtPath<InspectorGraphSettings>(AssetDatabase.GUIDToAssetPath(guids[0]));

            // Create default settings
            settings = CreateInstance<InspectorGraphSettings>();
            settings.EnsureDefaultFilters();
            settings.EnsureDefaultReferenceColors();
            settings.EnsureDefaultWindowSizes();
            Directory.CreateDirectory(Path.GetDirectoryName(kDefaultSettingsLocation));
            // Save
            AssetDatabase.CreateAsset(settings, kDefaultSettingsLocation);
            AssetDatabase.SaveAssets();
            return settings;
        }
    }
}
