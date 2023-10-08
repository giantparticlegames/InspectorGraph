// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using GiantParticle.InspectorGraph.UIToolkit;
using UnityEditor;
using UnityEngine.UIElements;

namespace GiantParticle.InspectorGraph.PropertyDrawers
{
    internal abstract class BasePropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();
            var asset = UIToolkitHelper.LocateViewForType(this);
            if (asset == null) return root;
            asset.CloneTree(root);
            UIToolkitHelper.ResolveVisualElements(this, root);

            ConfigureFields(property);
            return root;
        }

        protected abstract void ConfigureFields(SerializedProperty property);
    }
}
