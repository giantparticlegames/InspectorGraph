// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using Object = UnityEngine.Object;

namespace GiantParticle.InspectorGraph.Editor.Data.Nodes.ObjectNodeProcessors
{
    /// <summary>
    /// The AnimatorController structure is similar to a GameObject in the sense that it contains references to objects
    /// that are declared inside the same file, hence, we need to extract and scan all those objects to properly detect
    /// references to other files like AnimationClips.
    /// One main difference with other objects is that its references are all hidden.
    /// </summary>
    internal class AnimatorControllerObjectNodeProcessor : BaseObjectNodeProcessor
    {
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
    }
}
