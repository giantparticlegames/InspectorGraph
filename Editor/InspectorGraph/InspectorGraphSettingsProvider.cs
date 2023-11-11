// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System.Collections.Generic;
using GiantParticle.InspectorGraph.Persistence;
using GiantParticle.InspectorGraph.UIToolkit;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GiantParticle.InspectorGraph.Views
{
    internal class InspectorGraphSettingsProvider : SettingsProvider
    {
        public const string kMenuPath = "Giant Particle/Inspector Graph";

        [VisualElementField]
        private ScrollView _settingsContainer;

        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            return new InspectorGraphSettingsProvider(kMenuPath, SettingsScope.Project);
        }

        public InspectorGraphSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null) :
            base(path, scopes, keywords)
        {
        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            var asset = UIToolkitHelper.LocateViewForType(this);
            if (asset == null) return;
            asset.CloneTree(rootElement);
            UIToolkitHelper.ResolveVisualElements(this, rootElement);

            var settings = InspectorGraphProjectSettings.instance;
            settings.hideFlags &= ~HideFlags.NotEditable;
            var serialized = new SerializedObject(settings);

            var fields = settings.SerializedFieldNames;
            for (int i = 0; i < fields.Length; ++i)
            {
                var propertyField = new PropertyField();
                propertyField.bindingPath = fields[i];
                propertyField.Bind(serialized);
                _settingsContainer.Add(propertyField);
            }
        }
    }
}
