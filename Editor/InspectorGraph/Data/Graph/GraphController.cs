// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using GiantParticle.InspectorGraph.Data.Graph;
using GiantParticle.InspectorGraph.Data.Nodes;
using GiantParticle.InspectorGraph.Persistence;

namespace GiantParticle.InspectorGraph.Editor.InspectorGraph.Data.Graph
{
    internal class GraphController : IGraphController
    {
        public event Action<IGraphController> SelectedFactoryChanged;
        public IGraphFactory ActiveFactory => _currentFactory;
        public IGraphFactory[] AvailableFactories { get; }

        public IObjectNode ActiveGraph => _currentFactory.CurrentGraph;

        private IGraphFactory _currentFactory;

        public GraphController()
        {
            AvailableFactories = ReflectionHelper.InstantiateAllImplementations<IGraphFactory>();
            Array.Sort(AvailableFactories, ReflectionHelper.CompareByPriority);
            var preferences = GlobalApplicationContext.Instance.Get<IInspectorGraphUserPreferences>();

            if (SelectFactory(preferences.SelectedGraphFactoryIndex)) return;
            SelectFactory(0);
        }

        public void ClearActiveGraph() => _currentFactory.ClearGraph();

        public bool SelectFactory(int index)
        {
            if (index < 0 || AvailableFactories.Length <= index) return false;

            _currentFactory = AvailableFactories[index];
            var preferences = GlobalApplicationContext.Instance.Get<IInspectorGraphUserPreferences>();
            preferences.SelectedGraphFactoryIndex = index;
            preferences.Save();

            SelectedFactoryChanged?.Invoke(this);
            return true;
        }

        public bool SelectFactory(IGraphFactory factory)
        {
            for (int i = 0; i < AvailableFactories.Length; ++i)
            {
                if (AvailableFactories[i] != factory) continue;
                return SelectFactory(i);
            }

            return false;
        }
    }
}
