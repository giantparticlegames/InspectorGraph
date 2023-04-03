// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using System.Collections.Generic;
using GiantParticle.InspectorGraph.Editor.Data.Nodes.SerializedPropertyProcessors;
using UnityEditor;
using Object = UnityEngine.Object;

namespace GiantParticle.InspectorGraph.Editor.Data.Nodes.ObjectNodeProcessors
{
    internal abstract class BaseObjectNodeProcessor : IObjectNodeProcessor
    {
        public abstract Type TargetType { get; }
        protected IReadOnlyList<ISerializedPropertyProcessor> PropertyProcessors { get; private set; }

        public void SetPropertyProcessors(IReadOnlyList<ISerializedPropertyProcessor> processors)
        {
            PropertyProcessors = processors;
        }

        public abstract void ProcessNode(ObjectNode node);

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
            SerializedProperty iterator = serializedObject.GetIterator();
            while (iterator.NextVisible(true))
            {
                // Iterate over processors
                for (int i = 0; i < propertyProcessors.Count; ++i)
                {
                    var processor = propertyProcessors[i];
                    if (processor.ProcessSerializedProperty(iterator, node, null))
                        break;
                }
            }
        }
    }
}
