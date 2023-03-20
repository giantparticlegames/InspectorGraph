// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using GiantParticle.InspectorGraph.Editor.Common;
using GiantParticle.InspectorGraph.Settings;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GiantParticle.InspectorGraph.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(ReferenceColorSettings))]
    internal class ReferenceColorSettingsPropertyDrawer : BasePropertyDrawer
    {
        protected override UIDocumentTypes DocumentType => UIDocumentTypes.ReferenceColorSettings;

        protected override void CreateFields(VisualElement root, SerializedProperty property)
        {
            Label field = root.Q<Label>("_referenceTypeLabel");
            field.Bind(property.serializedObject);

            ColorField normalColorField = root.Q<ColorField>("_normalColorField");
            normalColorField.Bind(property.serializedObject);

            ColorField highlightedColorField = root.Q<ColorField>("_highlightedColorField");
            highlightedColorField.Bind(property.serializedObject);
        }
    }
}
