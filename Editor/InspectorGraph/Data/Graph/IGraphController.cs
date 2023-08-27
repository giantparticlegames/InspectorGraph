// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using GiantParticle.InspectorGraph.Data.Graph;

namespace GiantParticle.InspectorGraph.Editor.InspectorGraph.Data.Graph
{
    internal interface IGraphController
    {
        IGraphFactory ActiveFactory { get; }

        IGraphFactory[] AvailableFactories { get; }

        void SelectFactory(int index);
    }
}
