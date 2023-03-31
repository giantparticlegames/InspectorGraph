// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System.Collections.Generic;
using System.IO;
using GiantParticle.InspectorGraph.Editor.Data.Nodes;
using UnityEditor;
using UnityEngine;

namespace GiantParticle.InspectorGraph.Settings
{
    internal interface IInspectorGraphSettings
    {
        int MaxPreviewWindows { get; }
        int MaxWindows { get; }
        IReadOnlyList<IFilterTypeSettings> TypeFilters { get; }
        IReadOnlyList<IReferenceColorSettings> ReferenceColorSettings { get; }

        IReferenceColorSettings GetReferenceColor(ReferenceType type);
    }

    internal partial class InspectorGraphSettings : ScriptableObject, IInspectorGraphSettings
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

        public int MaxPreviewWindows => _maxPreviewWindows;
        public int MaxWindows => _maxWindows;
        public IReadOnlyList<IFilterTypeSettings> TypeFilters => _filters;
        public IReadOnlyList<IReferenceColorSettings> ReferenceColorSettings => _referenceColors;

        public IReferenceColorSettings GetReferenceColor(ReferenceType type)
        {
            // Assume the order is the same as the enum values
            return _referenceColors[type - 1];
        }

        private void OnEnable()
        {
            EnsureDefaultFilters();
            EnsureDefaultReferenceColors();
        }

        private void EnsureDefaultReferenceColors()
        {
            if (_referenceColors == null) _referenceColors = new List<ReferenceColorSettings>();
            if (_referenceColors.Count < ReferenceType.Direct)
            {
                _referenceColors.Add(new ReferenceColorSettings(
                    referenceType: ReferenceType.Direct,
                    normalColor: new Color(1, 1, 1, 0.5f),
                    highlightedColor: new Color(1, 1, 1, 1)));
            }

            if (_referenceColors.Count < ReferenceType.NestedPrefab)
            {
                _referenceColors.Add(new ReferenceColorSettings(
                    referenceType: ReferenceType.NestedPrefab,
                    normalColor: new Color(0, 1, 1, 0.5f),
                    highlightedColor: new Color(0, 1, 1, 1)));
            }

            AddExtendedReferenceColors();
        }

        partial void AddExtendedReferenceColors();

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
            Directory.CreateDirectory(Path.GetDirectoryName(kDefaultSettingsLocation));
            // Save
            AssetDatabase.CreateAsset(settings, kDefaultSettingsLocation);
            AssetDatabase.SaveAssets();
            return settings;
        }
    }
}
