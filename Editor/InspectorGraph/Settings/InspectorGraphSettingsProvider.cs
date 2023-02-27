// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GiantParticle.InspectorGraph.Settings
{
    public class InspectorGraphSettingsProvider : SettingsProvider
    {
        private VisualTreeAsset _settingsView;
        private InspectorGraphSettings _settings;

        public InspectorGraphSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null) :
            base(path, scopes, keywords)
        {
        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            _settings = InspectorGraphSettings.GetSettings();
            string layoutPath = AssetDatabase.GUIDToAssetPath("25e2a000771f04d6fa51874a8e2f4d42");
            _settingsView = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(layoutPath);

            _settingsView.CloneTree(rootElement);

            var serializedSettings = new SerializedObject(_settings);
            // Bind fields
            var maximumWindows = rootElement.Q<SliderInt>("_maxWindows");
            maximumWindows.bindingPath = nameof(_settings._maxWindows);
            maximumWindows.Bind(serializedSettings);

            var maximumPreviewWindows = rootElement.Q<SliderInt>("_maxPreviewWindows");
            maximumPreviewWindows.bindingPath = nameof(_settings._maxPreviewWindows);
            maximumPreviewWindows.Bind(serializedSettings);

            var filterListField = rootElement.Q<PropertyField>("_filterListField");
            filterListField.bindingPath = nameof(_settings._filters);
            filterListField.Bind(serializedSettings);
        }
    }
}
