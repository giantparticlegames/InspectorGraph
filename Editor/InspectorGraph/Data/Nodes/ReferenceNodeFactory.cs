// ********************************
// (C) 2022 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using System.Collections.Generic;
using GiantParticle.InspectorGraph.Editor.Data.Nodes.Filters;
using GiantParticle.InspectorGraph.Editor.Data.Nodes.ObjectNodeProcessors;
using GiantParticle.InspectorGraph.Editor.Data.Nodes.SerializedPropertyProcessors;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GiantParticle.InspectorGraph.Editor.Data.Nodes
{
    internal class ReferenceNodeFactory
    {
        private readonly Queue<ObjectNode> _queue = new();

        private readonly ITypeFilterHandler _typeFilter;
        private readonly List<ISerializedPropertyProcessor> _propertyProcessors;
        private readonly Dictionary<Type, IObjectNodeProcessor> _objectNodeProcessors;

        public ReferenceNodeFactory(ITypeFilterHandler filterHandler)
        {
            _typeFilter = filterHandler;
            _propertyProcessors = new List<ISerializedPropertyProcessor>();
            CreateAllSerializedPropertyProcessors();
            _objectNodeProcessors = new Dictionary<Type, IObjectNodeProcessor>();
            CreateAllObjectNodeProcessors();
        }

        private void CreateAllSerializedPropertyProcessors()
        {
            var processors = ReflectionHelper.InstantiateAllImplementations<ISerializedPropertyProcessor>();
            for (int i = 0; i < processors.Length; ++i)
            {
                var processor = processors[i];
                processor.FilterHandler = _typeFilter;
                processor.NodeQueue = _queue;
                _propertyProcessors.Add(processor);
            }

            // Sort by priority
            _propertyProcessors.Sort((processor, propertyProcessor) => processor.Priority - propertyProcessor.Priority);
        }

        private void CreateAllObjectNodeProcessors()
        {
            var processors = ReflectionHelper.InstantiateAllImplementations<IObjectNodeProcessor>();
            for (int i = 0; i < processors.Length; ++i)
            {
                var processor = processors[i];
                processor.SetPropertyProcessors(_propertyProcessors);
                if (processor.TargetType == null)
                {
                    Debug.LogError($"Object Node Processor [{processor.GetType()}] noes not have a target type");
                    continue;
                }

                if (_objectNodeProcessors.ContainsKey(processor.TargetType))
                {
                    Debug.LogError($"Duplicated Object Node Processor for type [{processor.TargetType}]");
                    continue;
                }

                _objectNodeProcessors.Add(processor.TargetType, processor);
            }
        }

        public IObjectNode CreateGraphFromObject(Object rootObject)
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

                if (!_typeFilter.ShouldExpandObject(node.Target))
                    continue;
                if (!_typeFilter.ShouldShowObject(node.Target))
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
