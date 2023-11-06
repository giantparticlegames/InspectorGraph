// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using GiantParticle.InspectorGraph.Persistence;
using GiantParticle.InspectorGraph.UIToolkit;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GiantParticle.InspectorGraph.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(InspectorWindowSettings))]
    internal class InspectorWindowSettingsPropertyDrawer : BasePropertyDrawer
    {
        [VisualElementField]
        private SliderInt _maxPreviewWindowsField;

        [VisualElementField]
        private SliderInt _maxWindowsField;

        [VisualElementField]
        private Foldout _defaultWindowSizes;

        protected override void ConfigureFields(SerializedProperty property)
        {
            var windowSettings = InspectorGraphProjectSettings.instance.WindowSettings;
            _maxWindowsField.bindingPath = nameof(InspectorWindowSettings._maxWindows);
            _maxWindowsField.Bind(property.serializedObject);
            _maxWindowsField.RegisterValueChangedCallback(evt => { InspectorGraphProjectSettings.instance.Save(); });

            _maxPreviewWindowsField.bindingPath = nameof(InspectorWindowSettings._maxPreviewWindows);
            _maxPreviewWindowsField.Bind(property.serializedObject);
            _maxPreviewWindowsField.RegisterValueChangedCallback(evt => { InspectorGraphProjectSettings.instance.Save(); });

            var sizesProperty = property.FindPropertyRelative(nameof(InspectorWindowSettings._windowSizeSettings));
            for (int i = 0; i < windowSettings.WindowSizeSettings.Count; ++i)
            {
                var propertyField = new PropertyField(sizesProperty.GetArrayElementAtIndex(i));
                propertyField.Bind(property.serializedObject);
                _defaultWindowSizes.Add(propertyField);
            }
        }
    }
}
