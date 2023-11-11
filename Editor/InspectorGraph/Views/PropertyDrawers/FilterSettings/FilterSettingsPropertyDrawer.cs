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
    [CustomPropertyDrawer(typeof(FilterSettings))]
    internal class FilterSettingsPropertyDrawer : BasePropertyDrawer
    {
        [VisualElementField]
        private Toggle _enableFilterToggle;

        [VisualElementField]
        private PropertyField _filterListField;

        protected override void ConfigureFields(SerializedProperty property)
        {
            _enableFilterToggle.bindingPath = nameof(FilterSettings._enableFilters);
            _enableFilterToggle.Bind(property.serializedObject);
            _enableFilterToggle.RegisterValueChangedCallback(evt => InspectorGraphProjectSettings.instance.Save());

            _filterListField.bindingPath = nameof(FilterSettings._typeFilters);
            _filterListField.Bind(property.serializedObject);
            _filterListField.RegisterValueChangeCallback(evt => InspectorGraphProjectSettings.instance.Save());
        }
    }
}
