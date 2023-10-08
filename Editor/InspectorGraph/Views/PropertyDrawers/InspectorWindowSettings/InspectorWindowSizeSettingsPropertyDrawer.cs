// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using GiantParticle.InspectorGraph.ContentView;
using GiantParticle.InspectorGraph.Persistence;
using GiantParticle.InspectorGraph.UIToolkit;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GiantParticle.InspectorGraph.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(InspectorWindowSizeSettings))]
    internal class InspectorWindowSizeSettingsPropertyDrawer : BasePropertyDrawer
    {
        [VisualElementField]
        private Label _modeField;

        [VisualElementField]
        private Vector2IntField _sizeField;

        protected override void ConfigureFields(SerializedProperty property)
        {
            var targetMode = property.FindPropertyRelative(nameof(InspectorWindowSizeSettings._targetMode));
            _modeField.text = $"{(ContentViewMode)targetMode.intValue}";

            _sizeField.bindingPath = nameof(InspectorWindowSizeSettings._targetSize);
            _sizeField.Bind(property.serializedObject);
            _sizeField.RegisterValueChangedCallback(evt => InspectorGraphProjectSettings.instance.Save());
        }
    }
}
