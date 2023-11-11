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
    [CustomPropertyDrawer(typeof(ReferenceColorSettings))]
    internal class ReferenceColorSettingsPropertyDrawer : BasePropertyDrawer
    {
        [VisualElementField]
        private Label _referenceTypeLabel;

        [VisualElementField]
        private ColorField _normalColorField;

        [VisualElementField]
        private ColorField _highlightedColorField;

        protected override void ConfigureFields(SerializedProperty property)
        {
            var propertyPath = $"{nameof(ReferenceColorSettings._referenceType)}.{nameof(ReferenceType.Value)}";
            var refType = property.FindPropertyRelative(propertyPath);
            var refTypeValue = new ReferenceType(refType.intValue);
            _referenceTypeLabel.text = refTypeValue.ToString();

            _normalColorField.bindingPath = nameof(ReferenceColorSettings._normalColor);
            _normalColorField.Bind(property.serializedObject);
            _normalColorField.RegisterValueChangedCallback(evt => InspectorGraphProjectSettings.instance.Save());

            _highlightedColorField.bindingPath = nameof(ReferenceColorSettings._highlightedColor);
            _highlightedColorField.Bind(property.serializedObject);
            _highlightedColorField.RegisterValueChangedCallback(evt => InspectorGraphProjectSettings.instance.Save());
        }
    }
}
