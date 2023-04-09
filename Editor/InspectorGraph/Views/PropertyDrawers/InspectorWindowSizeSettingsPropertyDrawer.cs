// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using GiantParticle.InspectorGraph.Editor.ContentView;
using GiantParticle.InspectorGraph.Editor.Settings;
using GiantParticle.InspectorGraph.Editor.UIDocuments;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GiantParticle.InspectorGraph.Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(InspectorWindowSizeSettings))]
    internal class InspectorWindowSizeSettingsPropertyDrawer : BasePropertyDrawer
    {
        protected override SettingsUIDocumentType DocumentType => SettingsUIDocumentType.WindowSizeSettings;

        protected override void CreateFields(VisualElement root, SerializedProperty property)
        {
            Label field = root.Q<Label>("_modeField");
            SerializedProperty targetMode = property.FindPropertyRelative("_targetMode");
            field.text = $"{(ContentViewMode)targetMode.intValue}";

            Vector2IntField sizeField = root.Q<Vector2IntField>("_sizeField");
            sizeField.Bind(property.serializedObject);
        }
    }
}
