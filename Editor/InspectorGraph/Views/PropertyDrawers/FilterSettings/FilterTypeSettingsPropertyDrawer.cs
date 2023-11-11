// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using GiantParticle.InspectorGraph.Persistence;
using GiantParticle.InspectorGraph.UIToolkit;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using PopupWindow = UnityEditor.PopupWindow;

namespace GiantParticle.InspectorGraph.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(FilterTypeSetting))]
    internal class FilterTypeSettingsPropertyDrawer : BasePropertyDrawer
    {
        [VisualElementField]
        private TextField _fullyQualifiedName;

        [VisualElementField]
        private Toggle _showType;

        [VisualElementField]
        private Toggle _expandType;

        protected override void ConfigureFields(SerializedProperty property)
        {
            _fullyQualifiedName.bindingPath = nameof(FilterTypeSetting._fullyQualifiedName);
            _fullyQualifiedName.Bind(property.serializedObject);
            _fullyQualifiedName.RegisterValueChangedCallback(evt => { InspectorGraphProjectSettings.instance.Save(); });
            _fullyQualifiedName.RegisterCallback<ClickEvent>(evt =>
            {
                PopupWindow.Show(
                    activatorRect: new Rect(evt.position, Vector2.zero),
                    windowContent: new FilterTypeOptionsPopup(
                        options: ReflectionHelper.ListAllTypes(),
                        onSelection: type => _fullyQualifiedName.value = type.FullName));
            });

            _showType.bindingPath = nameof(FilterTypeSetting._showType);
            _showType.Bind(property.serializedObject);
            _showType.RegisterValueChangedCallback(evt => { InspectorGraphProjectSettings.instance.Save(); });

            _expandType.bindingPath = nameof(FilterTypeSetting._expandType);
            _expandType.Bind(property.serializedObject);
            _expandType.RegisterValueChangedCallback(evt => { InspectorGraphProjectSettings.instance.Save(); });
        }
    }
}
