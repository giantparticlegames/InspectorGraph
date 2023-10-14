// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using GiantParticle.InspectorGraph.Data.Graph;
using GiantParticle.InspectorGraph.Data.Nodes;

namespace GiantParticle.InspectorGraph.Editor.InspectorGraph.Data.Graph
{
    internal interface IGraphController
    {
        event Action<IGraphController> SelectedFactoryChanged;
        IGraphFactory ActiveFactory { get; }

        IGraphFactory[] AvailableFactories { get; }

        IObjectNode ActiveGraph { get; }

        void ClearActiveGraph();

        bool SelectFactory(int index);
        bool SelectFactory(IGraphFactory factory);
    }
}
