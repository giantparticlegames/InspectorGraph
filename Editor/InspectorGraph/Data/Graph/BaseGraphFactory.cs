// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using GiantParticle.InspectorGraph.Data.Graph.Filters;
using GiantParticle.InspectorGraph.Data.Nodes;
using UnityEngine;

namespace GiantParticle.InspectorGraph.Data.Graph
{
    internal abstract class BaseGraphFactory : IGraphFactory
    {
        // TODO: Move this to an Attribute
        public abstract int DisplayPriority { get; }
        public abstract string DisplayName { get; }
        protected ITypeFilterHandler TypeFilter => GlobalApplicationContext.Instance.Get<ITypeFilterHandler>();
        public abstract IObjectNode CreateGraphFromObject(Object rootObject);
    }
}
