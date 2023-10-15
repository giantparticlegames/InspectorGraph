// ********************************
// (C) 2022 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using System.Collections.Generic;
using GiantParticle.InspectorGraph.CustomAttributes;
using GiantParticle.InspectorGraph.Data.Graph;
using GiantParticle.InspectorGraph.Data.Graph.SubTree.ObjectNodeProcessors;
using GiantParticle.InspectorGraph.Data.Graph.SubTree.SerializedPropertyProcessors;
using GiantParticle.InspectorGraph.Data.Nodes;
using GiantParticle.InspectorGraph.Operations;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GiantParticle.InspectorGraph.Data
{
    [InternalPriority(0)]
    [EditorDisplayName("References From Object")]
    internal class SubTreeGraphFactory : BaseGraphFactory
    {
        public override ReferenceDirection GraphDirection => ReferenceDirection.ReferenceTo;

        private readonly Queue<ObjectNode> _queue = new();
        private readonly IReadOnlyList<ISerializedPropertyProcessor> _propertyProcessors;
        private readonly IReadOnlyDictionary<Type, IObjectNodeProcessor> _objectNodeProcessors;

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
            processors.Sort(ReflectionHelper.CompareByPriority);
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

        public override IOperation<IObjectNode> CreateGraphFromObject(Object rootObject)
        {
            var nodeFactory = GlobalApplicationContext.Instance.Get<IObjectNodeFactory>();
            nodeFactory.ClearRegistry();
            _queue.Enqueue(nodeFactory.CreateNode(rootObject));

            Dictionary<Object, ObjectNode> visitedObjects = new();
            ObjectNode root = null;
            while (_queue.Count > 0)
            {
                var node = _queue.Dequeue();

                if (visitedObjects.ContainsKey(node.Object)) continue;

                if (root == null) root = node;
                visitedObjects.Add(node.Object, node);

                if (!TypeFilter.ShouldExpandObject(node.Object))
                    continue;
                if (!TypeFilter.ShouldShowObject(node.Object))
                    continue;

                // Process Object
                Type objectType = node.Object.GetType();
                if (_objectNodeProcessors.ContainsKey(objectType)) _objectNodeProcessors[objectType].ProcessNode(node);
                else BaseObjectNodeProcessor.ProcessSerializedProperties(_propertyProcessors, node);
            }

            CurrentGraph = root;
            return new Operation<IObjectNode>() { Progress = 1, Result = root, State = OperationState.Finished };
        }
    }
}
