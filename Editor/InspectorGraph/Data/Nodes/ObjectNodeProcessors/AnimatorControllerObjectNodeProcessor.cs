// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Animations;
using Object = UnityEngine.Object;

namespace GiantParticle.InspectorGraph.Editor.Data.Nodes.ObjectNodeProcessors
{
    /// <summary>
    /// The AnimatorController structure is similar to a GameObject in the sense that it contains references to objects
    /// that are declared inside the same file, hence, we need to extract and scan all those objects to properly detect
    /// references to other files like AnimationClips.
    /// </summary>
    internal class AnimatorControllerObjectNodeProcessor : BaseObjectNodeProcessor
    {
        private readonly Regex pointerRegex = new Regex("PPtr<.*>", RegexOptions.Compiled);
        public override Type TargetType => typeof(AnimatorController);

        public override void ProcessNode(ObjectNode node)
        {
            SerializedObject serializedObject = node.WindowData.SerializedTarget;

            Queue<SerializedObject> queue = new Queue<SerializedObject>();
            HashSet<Object> internalReferences = CreateInternalReferenceSet(serializedObject);
            foreach (Object internalObject in internalReferences)
                queue.Enqueue(new SerializedObject(internalObject));

            // Scan all internal objects
            while (queue.Count > 0)
            {
                ProcessAllSerializedProperties(
                    parentNode: node,
                    serializedObject: queue.Dequeue(),
                    excludeReferences: internalReferences);
            }
        }

        private HashSet<Object> CreateInternalReferenceSet(SerializedObject serializedObject)
        {
            var objectSet = new HashSet<Object>();
            objectSet.Add(serializedObject.targetObject);

            string objectPath = AssetDatabase.GetAssetPath(serializedObject.targetObject);
            if (string.IsNullOrEmpty(objectPath))
                return objectSet;

            Queue<SerializedObject> objectQueue = new Queue<SerializedObject>();
            objectQueue.Enqueue(serializedObject);

            while (objectQueue.Count > 0)
            {
                var currentObject = objectQueue.Dequeue();
                // Iterate fields
                var refIterator = currentObject.GetIterator();
                while (refIterator.Next(true))
                {
                    // Check only PPtr<*> types, otherwise we will see errors in the console
                    if (!pointerRegex.IsMatch(refIterator.type)) continue;

                    var objReference = refIterator.objectReferenceValue;
                    if (objReference == null) continue;
                    if (objectSet.Contains(objReference)) continue;

                    var refPath = AssetDatabase.GetAssetPath(objReference);
                    if (string.IsNullOrEmpty(refPath)) continue;
                    if (!string.Equals(objectPath, refPath)) continue;

                    objectSet.Add(objReference);
                    objectQueue.Enqueue(new SerializedObject(objReference));
                }
            }

            return objectSet;
        }
    }
}
