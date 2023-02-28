// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System.Collections.Generic;
using GiantParticle.InspectorGraph.Editor.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GiantParticle.InspectorGraph.Settings
{
    public class InspectorGraphSettingsProvider : SettingsProvider
    {
        public InspectorGraphSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null) :
            base(path, scopes, keywords)
        {
        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            var settings = InspectorGraphSettings.GetSettings();
            var settingsLayout = UIDocumentCatalog.GetCatalog()[UIDocumentTypes.Settings].Asset;

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
        }
    }
}
