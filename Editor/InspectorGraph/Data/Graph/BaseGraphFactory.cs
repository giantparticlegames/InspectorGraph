// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using GiantParticle.InspectorGraph.Data.Graph.Filters;
using GiantParticle.InspectorGraph.Data.Nodes;
using GiantParticle.InspectorGraph.Operations;
using Object = UnityEngine.Object;

namespace GiantParticle.InspectorGraph.Data.Graph
{
    internal abstract class BaseGraphFactory : IGraphFactory
    {
        public IObjectNode CurrentGraph { get; protected set; }
        public abstract ReferenceDirection GraphDirection { get; }

        public void ClearGraph()
        {
            CurrentGraph = null;
        }

        protected ITypeFilterHandler TypeFilter => GlobalApplicationContext.Instance.Get<ITypeFilterHandler>();
        public abstract IOperation<IObjectNode> CreateGraphFromObject(Object rootObject);
    }
}
