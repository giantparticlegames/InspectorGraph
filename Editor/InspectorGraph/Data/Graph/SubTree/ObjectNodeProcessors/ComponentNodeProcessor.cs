// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using System.Collections.Generic;
using GiantParticle.InspectorGraph.Data.Nodes;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GiantParticle.InspectorGraph.Data.Graph.SubTree.ObjectNodeProcessors
{
    internal class ComponentNodeProcessor : BaseObjectNodeProcessor
    {
        public override Type TargetType => typeof(Component);

        public override void ProcessNode(ObjectNode node)
        {
            SerializedObject serializedObject = node.WindowData.SerializedObject;

            Queue<SerializedObject> queue = new();
            HashSet<Object> internalReferences = CreateInternalReferenceSet(serializedObject, true);
            foreach (Object internalObject in internalReferences)
                queue.Enqueue(new SerializedObject(internalObject));

            // Scan all internal objects
            while (queue.Count > 0)
            {
                ProcessSerializedProperties(
                    onlyVisible: true,
                    parentNode: node,
                    serializedObject: queue.Dequeue(),
                    excludeReferences: internalReferences);
            }
        }
    }
}
