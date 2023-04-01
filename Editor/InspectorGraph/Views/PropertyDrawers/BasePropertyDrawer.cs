// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using GiantParticle.InspectorGraph.Editor.UIDocuments;
using UnityEditor;
using UnityEngine.UIElements;

namespace GiantParticle.InspectorGraph.Editor.PropertyDrawers
{
    internal abstract class BasePropertyDrawer : PropertyDrawer
    {
        protected abstract SettingsUIDocumentType DocumentType { get; }

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();
            var layout = SettingsUIDocumentCatalog.GetCatalog()[DocumentType].Asset;
            layout.CloneTree(root);
            CreateFields(root, property);

            return root;
        }

        protected abstract void CreateFields(VisualElement root, SerializedProperty property);
    }
}
