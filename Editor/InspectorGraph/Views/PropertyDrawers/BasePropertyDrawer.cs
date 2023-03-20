// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using GiantParticle.InspectorGraph.Editor.Common;
using UnityEditor;
using UnityEngine.UIElements;

namespace GiantParticle.InspectorGraph.PropertyDrawers
{
    internal abstract class BasePropertyDrawer : PropertyDrawer
    {
        protected abstract UIDocumentTypes DocumentType { get; }

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();
            var layout = UIDocumentCatalog.GetCatalog()[DocumentType].Asset;
            layout.CloneTree(root);
            CreateFields(root, property);

            return root;
        }

        protected abstract void CreateFields(VisualElement root, SerializedProperty property);
    }
}
