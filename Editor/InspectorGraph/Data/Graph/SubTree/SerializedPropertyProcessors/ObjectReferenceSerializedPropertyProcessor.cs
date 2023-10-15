// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System.Collections.Generic;
using GiantParticle.InspectorGraph.CustomAttributes;
using GiantParticle.InspectorGraph.Data.Nodes;
using UnityEditor;
using UnityEngine;

namespace GiantParticle.InspectorGraph.Data.Graph.SubTree.SerializedPropertyProcessors
{
    [InternalPriority(0)]
    internal class ObjectReferenceSerializedPropertyProcessor : BaseSerializedPropertyProcessor
    {
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
                string assetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(reference);
                if (!string.IsNullOrEmpty(assetPath))
                    reference = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
            }

            ObjectNode childNode = NodeFactory.CreateNode(reference);
            ObjectNode.CreateReference(
                sourceObject: parentNode,
                targetObject: childNode,
                referenceType: ReferenceType.Direct);

            // Expand if indicated
            if (!FilterHandler.ShouldExpandObject(reference))
                return true;
            NodeQueue.Enqueue(childNode);
            return true;
        }
    }
}
