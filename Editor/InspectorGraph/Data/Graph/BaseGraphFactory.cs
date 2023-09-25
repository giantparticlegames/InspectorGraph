// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using GiantParticle.InspectorGraph.Data.Graph.Filters;
using GiantParticle.InspectorGraph.Data.Nodes;
using GiantParticle.InspectorGraph.Operations;
using UnityEditor;
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

        protected string GetAssetPath(Object rootObject)
        {
            string targetPath = AssetDatabase.GetAssetPath(rootObject);
            if (!string.IsNullOrEmpty(targetPath)) return targetPath;

            targetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(rootObject);
            if (!string.IsNullOrEmpty(targetPath)) return targetPath;

            return null;
        }
    }
}
