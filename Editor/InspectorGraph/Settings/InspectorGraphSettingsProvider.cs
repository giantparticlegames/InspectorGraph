// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System.Collections.Generic;
using GiantParticle.InspectorGraph.Editor.UIDocuments;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GiantParticle.InspectorGraph.Editor.Settings
{
    internal class InspectorGraphSettingsProvider : SettingsProvider
    {
        public InspectorGraphSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null) :
            base(path, scopes, keywords)
        {
        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            var settings = InspectorGraphSettings.GetSettings();
            var settingsLayout = SettingsUIDocumentCatalog.GetCatalog()[SettingsUIDocumentType.SettingsPanel].Asset;

            settingsLayout.CloneTree(rootElement);

            var serializedSettings = new SerializedObject(settings);
            // Bind fields
            var maximumWindows = rootElement.Q<SliderInt>("_maxWindows");
            maximumWindows.bindingPath = nameof(settings._maxWindows);
            maximumWindows.Bind(serializedSettings);

            var maximumPreviewWindows = rootElement.Q<SliderInt>("_maxPreviewWindows");
            maximumPreviewWindows.bindingPath = nameof(settings._maxPreviewWindows);
            maximumPreviewWindows.Bind(serializedSettings);

            var filterListField = rootElement.Q<PropertyField>("_filterListField");
            filterListField.bindingPath = nameof(settings._filters);
            filterListField.Bind(serializedSettings);

            var colorsContainer = rootElement.Q<Foldout>("_referenceColorsContainer");
            var refColorsSerializedProperty = serializedSettings.FindProperty(nameof(settings._referenceColors));
            for (int i = 0; i < settings.ReferenceColorSettings.Count; ++i)
            {
                var propertyField = new PropertyField(refColorsSerializedProperty.GetArrayElementAtIndex(i));
                propertyField.Bind(serializedSettings);
                colorsContainer.Add(propertyField);
            }

            var defaultSizesList = rootElement.Q<Foldout>("_defaultWindowSizes");
            var windowSizesSerializedProperty = serializedSettings.FindProperty(nameof(settings._defaultWindowSize));
            for (int i = 0; i < settings.InspectorWindowSizeSettings.Count; ++i)
            {
                var propertyField = new PropertyField(windowSizesSerializedProperty.GetArrayElementAtIndex(i));
                propertyField.Bind(serializedSettings);
                defaultSizesList.Add(propertyField);
            }
        }
    }
}
