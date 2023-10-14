// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using GiantParticle.InspectorGraph.Data.Nodes;
using GiantParticle.InspectorGraph.Persistence;
using GiantParticle.InspectorGraph.UIToolkit;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GiantParticle.InspectorGraph.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(ConnectionSettings))]
    internal class ConnectionSettingsPropertyDrawer : BasePropertyDrawer
    {
        [VisualElementField]
        private Toggle _drawReferenceCountToggle;

        [VisualElementField]
        private Foldout _referenceColorsContainer;

        protected override void ConfigureFields(SerializedProperty property)
        {
            _drawReferenceCountToggle.bindingPath = nameof(ConnectionSettings._drawReferenceCount);
            _drawReferenceCountToggle.Bind(property.serializedObject);
            _drawReferenceCountToggle.RegisterValueChangedCallback(evt =>
                InspectorGraphProjectSettings.instance.Save());

            var connectionSettings = InspectorGraphProjectSettings.instance.ConnectionSettings;
            var sizesProperty = property.FindPropertyRelative(nameof(ConnectionSettings._colorSettings));
            for (int i = 0; i < connectionSettings.ColorSettings.Count; ++i)
            {
                var setting = connectionSettings.ColorSettings[i];
                if (string.Equals(setting.ReferenceType.ToString(), ReferenceType.kNotAvailable)) continue;
                var propertyField = new PropertyField(sizesProperty.GetArrayElementAtIndex(i));
                propertyField.Bind(property.serializedObject);
                _referenceColorsContainer.Add(propertyField);
            }
        }
    }
}
