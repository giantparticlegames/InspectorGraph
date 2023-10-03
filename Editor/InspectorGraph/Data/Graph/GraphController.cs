// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using GiantParticle.InspectorGraph.Data.Graph;
using GiantParticle.InspectorGraph.Data.Nodes;

namespace GiantParticle.InspectorGraph.Editor.InspectorGraph.Data.Graph
{
    internal class GraphController : IGraphController
    {
        private IGraphFactory _currentFactory;
        public IGraphFactory ActiveFactory => _currentFactory;
        public IGraphFactory[] AvailableFactories { get; }

        public IObjectNode ActiveGraph => _currentFactory.CurrentGraph;

        public GraphController()
        {
            AvailableFactories = ReflectionHelper.InstantiateAllImplementations<IGraphFactory>();
            Array.Sort(AvailableFactories, ReflectionHelper.CompareByPriority);
            _currentFactory = AvailableFactories[0];
        }

        public void ClearActiveGraph() => _currentFactory.ClearGraph();

        public void SelectFactory(int index)
        {
            _currentFactory = AvailableFactories[index];
        }

        public void SelectFactory(IGraphFactory factory)
        {
            _currentFactory = factory;
        }
    }
}
