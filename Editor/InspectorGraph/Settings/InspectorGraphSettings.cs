// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GiantParticle.InspectorGraph.Settings
{
    internal interface IInspectorGraphSettings
    {
        int MaxPreviewWindows { get; }
        int MaxWindows { get; }
        IReadOnlyList<FilterTypeSettings> TypeFilters { get; }
    }

    internal class InspectorGraphSettings : ScriptableObject, IInspectorGraphSettings
    {
        public const string kDefaultSettingsLocation =
            "Assets/Editor/com.giantparticle.inspector_graph/InspectorGraphSettings.asset";

        [SerializeField]
        internal int _maxPreviewWindows = 50;

        [SerializeField]
        internal int _maxWindows = 100;

        [SerializeField]
        internal List<FilterTypeSettings> _filters;

        public int MaxPreviewWindows => _maxPreviewWindows;
        public int MaxWindows => _maxWindows;
        public IReadOnlyList<FilterTypeSettings> TypeFilters => _filters;

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
            settings._filters = new List<FilterTypeSettings>
            {
                new FilterTypeSettings()
                {
                    FullyQualifiedName = "UnityEngine.GameObject", ShowType = true, ExpandType = false
                },
                new FilterTypeSettings()
                {
                    FullyQualifiedName = "UnityEditor.MonoScript", ShowType = false, ExpandType = false
                },
                new FilterTypeSettings()
                {
                    FullyQualifiedName = "UnityEngine.U2D.SpriteAtlas", ShowType = true, ExpandType = false
                },
            };
            Directory.CreateDirectory(Path.GetDirectoryName(kDefaultSettingsLocation));
            // Save
            AssetDatabase.CreateAsset(settings, kDefaultSettingsLocation);
            AssetDatabase.SaveAssets();
            return settings;
        }
    }
}
