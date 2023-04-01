// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GiantParticle.InspectorGraph.Editor.Data.Nodes.SerializedPropertyProcessors
{
    internal class ObjectReferenceSerializedPropertyProcessor : BaseSerializedPropertyProcessor
    {
        public override int Priority => 0;

        public override bool ProcessSerializedProperty(SerializedProperty property, ObjectNode parentNode,
            ICollection<Object> excludeSet)
        {
            if (property.propertyType != SerializedPropertyType.ObjectReference)
                return false;

            // From this point onwards, the property declared processed
            Object reference = property.objectReferenceValue;
            if (reference == null)
                return true;
            if (!FilterHandler.ShouldShowObject(reference))
                return true;
            if (excludeSet != null && excludeSet.Contains(reference))
                return true;

            // Translate component reference to Prefab
            if (reference is Component)
            {
                string prefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(reference);
                if (!string.IsNullOrEmpty(prefabPath))
                    reference = AssetDatabase.LoadAssetAtPath<Object>(prefabPath);
            }

            ObjectNode childNode = new ObjectNode(new WindowData(reference));
            parentNode.AddNode(childNode, ReferenceType.Direct);

            // Expand if indicated
            if (!FilterHandler.ShouldExpandObject(reference))
                return true;
            NodeQueue.Enqueue(childNode);
            return true;
        }
    }
}
