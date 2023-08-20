// ********************************
// (C) 2022 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using System.Collections.Generic;
using GiantParticle.InspectorGraph.Data.Graph;
using GiantParticle.InspectorGraph.Data.Graph.SubTree.ObjectNodeProcessors;
using GiantParticle.InspectorGraph.Data.Graph.SubTree.SerializedPropertyProcessors;
using GiantParticle.InspectorGraph.Data.Nodes;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GiantParticle.InspectorGraph.Data
{
    internal class SubTreeGraphFactory : BaseGraphFactory
    {
        private readonly Queue<ObjectNode> _queue = new();

        private readonly IReadOnlyList<ISerializedPropertyProcessor> _propertyProcessors;
        private readonly IReadOnlyDictionary<Type, IObjectNodeProcessor> _objectNodeProcessors;

        public override int DisplayPriority => 0;
        public override string DisplayName => "References From Object";

        public SubTreeGraphFactory()
        {
            _propertyProcessors = CreateAllSerializedPropertyProcessors();
            _objectNodeProcessors = CreateAllObjectNodeProcessors();
        }

        private IReadOnlyList<ISerializedPropertyProcessor> CreateAllSerializedPropertyProcessors()
        {
            List<ISerializedPropertyProcessor> processors = new();
            var processorInstances = ReflectionHelper.InstantiateAllImplementations<ISerializedPropertyProcessor>();
            for (int i = 0; i < processorInstances.Length; ++i)
            {
                var instance = processorInstances[i];
                instance.NodeQueue = _queue;
                processors.Add(instance);
            }

            // Sort by priority
            processors.Sort((processor, propertyProcessor) => processor.Priority - propertyProcessor.Priority);
            return processors;
        }

        private IReadOnlyDictionary<Type, IObjectNodeProcessor> CreateAllObjectNodeProcessors()
        {
            Dictionary<Type, IObjectNodeProcessor> objectNodeProcessors = new();
            var processorInstances = ReflectionHelper.InstantiateAllImplementations<IObjectNodeProcessor>();
            for (int i = 0; i < processorInstances.Length; ++i)
            {
                var instance = processorInstances[i];
                instance.SetPropertyProcessors(_propertyProcessors);
                if (instance.TargetType == null)
                {
                    Debug.LogError($"Object Node Processor [{instance.GetType()}] noes not have a target type");
                    continue;
                }

                if (objectNodeProcessors.ContainsKey(instance.TargetType))
                {
                    Debug.LogError($"Duplicated Object Node Processor for type [{instance.TargetType}]");
                    continue;
                }

                objectNodeProcessors.Add(instance.TargetType, instance);
            }

            return objectNodeProcessors;
        }

        public override IObjectNode CreateGraphFromObject(Object rootObject)
        {
            _queue.Enqueue(new ObjectNode(new WindowData(rootObject)));

            Dictionary<Object, ObjectNode> visitedObjects = new();
            ObjectNode root = null;
            while (_queue.Count > 0)
            {
                var node = _queue.Dequeue();

                if (visitedObjects.ContainsKey(node.Target)) continue;

                if (root == null) root = node;
                visitedObjects.Add(node.Target, node);

                if (!TypeFilter.ShouldExpandObject(node.Target))
                    continue;
                if (!TypeFilter.ShouldShowObject(node.Target))
                    continue;

                // Process Object
                Type objectType = node.Target.GetType();
                if (_objectNodeProcessors.ContainsKey(objectType)) _objectNodeProcessors[objectType].ProcessNode(node);
                else BaseObjectNodeProcessor.ProcessSerializedProperties(_propertyProcessors, node);
            }

            return root;
        }
    }
}
