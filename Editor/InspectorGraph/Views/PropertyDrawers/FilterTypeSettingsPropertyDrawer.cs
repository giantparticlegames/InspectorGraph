// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using GiantParticle.InspectorGraph.Editor.Common;
using GiantParticle.InspectorGraph.Settings;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using PopupWindow = UnityEditor.PopupWindow;
using GiantParticle.InspectorGraph.Editor.Common.Utils;

namespace GiantParticle.InspectorGraph.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(FilterTypeSettings))]
    internal class FilterTypeSettingsPropertyDrawer : BasePropertyDrawer
    {
        protected override UIDocumentTypes DocumentType => UIDocumentTypes.FilterTypeSettings;

        protected override void CreateFields(VisualElement root, SerializedProperty property)
        {
            TextField field = root.Q<TextField>("_fullyQualifiedName");
            field.Bind(property.serializedObject);

            field.RegisterCallback<ClickEvent>(evt =>
            {
                PopupWindow.Show(
                    new Rect(evt.position, Vector2.zero),
                    new TypeOptionsPopup(ReflectionHelper.ListAllTypes(), type => field.value = type.FullName));
            });

            Toggle showToggle = root.Q<Toggle>("_showType");
            showToggle.Bind(property.serializedObject);

            Toggle expandToggle = root.Q<Toggle>("_expandType");
            expandToggle.Bind(property.serializedObject);
        }
    }
}
