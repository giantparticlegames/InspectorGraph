// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using GiantParticle.InspectorGraph.CustomAttributes;
using GiantParticle.InspectorGraph.Data.Graph;

namespace GiantParticle.InspectorGraph.Editor.InspectorGraph.Data.Graph
{
    internal class GraphController : IGraphController
    {
        private IGraphFactory _currentFactory;
        public IGraphFactory ActiveFactory => _currentFactory;
        public IGraphFactory[] AvailableFactories { get; }

        public GraphController()
        {
            AvailableFactories = ReflectionHelper.InstantiateAllImplementations<IGraphFactory>();
            Type priorityAttributeType = typeof(EditorDisplayPriorityAttribute);
            Array.Sort(AvailableFactories, (factoryA, factoryB) =>
            {
                var priorityA = (EditorDisplayPriorityAttribute)Attribute.GetCustomAttribute(
                    element: factoryA.GetType(),
                    attributeType: priorityAttributeType);
                var priorityB = (EditorDisplayPriorityAttribute)Attribute.GetCustomAttribute(
                    element: factoryB.GetType(),
                    attributeType: priorityAttributeType);
                return priorityA.Priority - priorityB.Priority;
            });
            _currentFactory = AvailableFactories[0];
        }

        public void SelectFactory(int index)
        {
            _currentFactory = AvailableFactories[index];
        }
    }
}
