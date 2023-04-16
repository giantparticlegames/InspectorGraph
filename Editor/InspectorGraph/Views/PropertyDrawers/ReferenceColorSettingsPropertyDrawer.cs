// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using GiantParticle.InspectorGraph.Editor.UIDocuments;
using GiantParticle.InspectorGraph.Editor.Data.Nodes;
using GiantParticle.InspectorGraph.Editor.Settings;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GiantParticle.InspectorGraph.Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(ReferenceColorSettings))]
    internal class ReferenceColorSettingsPropertyDrawer : BasePropertyDrawer
    {
        protected override SettingsUIDocumentType DocumentType => SettingsUIDocumentType.ReferenceColorSettings;

        protected override void CreateFields(VisualElement root, SerializedProperty property)
        {
            Label field = root.Q<Label>("_referenceTypeLabel");
            SerializedProperty refType = property.FindPropertyRelative("_referenceType.Value");
            var refTypeValue = new ReferenceType(refType.intValue);
            field.text = refTypeValue.ToString();

            ColorField normalColorField = root.Q<ColorField>("_normalColorField");
            normalColorField.Bind(property.serializedObject);

            ColorField highlightedColorField = root.Q<ColorField>("_highlightedColorField");
            highlightedColorField.Bind(property.serializedObject);
        }
    }
}
