// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using GiantParticle.InspectorGraph.Editor.Data.Nodes.SerializedPropertyProcessors;
using UnityEditor;
using Object = UnityEngine.Object;

namespace GiantParticle.InspectorGraph.Editor.Data.Nodes.ObjectNodeProcessors
{
    internal abstract class BaseObjectNodeProcessor : IObjectNodeProcessor
    {
        private const string kBuiltInResources = "Resources";
        private static readonly Regex PointerRegex = new Regex("PPtr<.*>", RegexOptions.Compiled);

        public abstract Type TargetType { get; }
        protected IReadOnlyList<ISerializedPropertyProcessor> PropertyProcessors { get; private set; }

        public void SetPropertyProcessors(IReadOnlyList<ISerializedPropertyProcessor> processors)
        {
            PropertyProcessors = processors;
        }

        public abstract void ProcessNode(ObjectNode node);

        protected static HashSet<Object> CreateInternalReferenceSet(SerializedObject serializedObject)
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
                    if (!PointerRegex.IsMatch(refIterator.type)) continue;

                    var objReference = refIterator.objectReferenceValue;
                    if (objReference == null) continue;
                    if (objectSet.Contains(objReference)) continue;

                    var refPath = AssetDatabase.GetAssetPath(objReference);
                    if (string.IsNullOrEmpty(refPath)) continue;
                    // If it's a built-in resource we do not consider any as internal reference
                    if (refPath.StartsWith(kBuiltInResources)) continue;
                    // If the asset path is different than the parent, is not an internal reference
                    if (!string.Equals(objectPath, refPath)) continue;

                    objectSet.Add(objReference);
                    objectQueue.Enqueue(new SerializedObject(objReference));
                }
            }

            return objectSet;
        }

        protected void ProcessAllVisibleSerializedProperties(ObjectNode parentNode, SerializedObject serializedObject,
            HashSet<Object> excludeReferences = null)
        {
            ProcessSerializedProperties(
                onlyVisible: true,
                parentNode: parentNode,
                serializedObject: serializedObject,
                excludeReferences: excludeReferences);
        }

        protected void ProcessAllSerializedProperties(ObjectNode parentNode, SerializedObject serializedObject,
            HashSet<Object> excludeReferences = null)
        {
            ProcessSerializedProperties(
                onlyVisible: false,
                parentNode: parentNode,
                serializedObject: serializedObject,
                excludeReferences: excludeReferences);
        }

        private void ProcessSerializedProperties(bool onlyVisible, ObjectNode parentNode,
            SerializedObject serializedObject,
            HashSet<Object> excludeReferences = null)
        {
            // Scan direct references
            SerializedProperty iterator = serializedObject.GetIterator();
            while (onlyVisible
                       ? iterator.NextVisible(true)
                       : iterator.Next(true))
            {
                // Iterate over processors
                for (int i = 0; i < PropertyProcessors.Count; ++i)
                {
                    var processor = PropertyProcessors[i];
                    if (processor.ProcessSerializedProperty(iterator, parentNode, excludeReferences))
                        break;
                }
            }
        }

        public static void ProcessSerializedProperties(IReadOnlyList<ISerializedPropertyProcessor> propertyProcessors,
            ObjectNode node)
        {
            SerializedObject serializedObject = node.WindowData.SerializedTarget;

            Queue<SerializedObject> queue = new Queue<SerializedObject>();
            HashSet<Object> internalReferences = CreateInternalReferenceSet(serializedObject);
            foreach (Object internalObject in internalReferences)
                queue.Enqueue(new SerializedObject(internalObject));

            // Scan all internal objects
            while (queue.Count > 0)
            {
                SerializedObject currentObject = queue.Dequeue();
                // Scan all internal values
                SerializedProperty iterator = currentObject.GetIterator();
                while (iterator.NextVisible(true))
                {
                    // Iterate over processors
                    for (int i = 0; i < propertyProcessors.Count; ++i)
                    {
                        var processor = propertyProcessors[i];
                        if (processor.ProcessSerializedProperty(iterator, node, internalReferences))
                            break;
                    }
                }
            }
        }
    }
}
