// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GiantParticle.InspectorGraph.Data.Nodes
{
    internal class ObjectNodeFactory : IObjectNodeFactory
    {
        private Dictionary<Object, ObjectNode> _nodes = new();
        private Dictionary<string, ObjectNode> _nodesByAssetPath = new();

        public void ClearRegistry()
        {
            _nodes.Clear();
            _nodesByAssetPath.Clear();
        }

        public ObjectNode CreateNode(Object obj, bool skipPath = false)
        {
            if (!skipPath)
            {
                var assetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(obj);
                if (!string.IsNullOrEmpty(assetPath))
                {
                    if (_nodesByAssetPath.ContainsKey(assetPath)) return _nodesByAssetPath[assetPath];
                    var nodeAsset = new ObjectNode(obj);
                    _nodesByAssetPath.Add(assetPath, nodeAsset);
                    return nodeAsset;
                }
            }

            if (_nodes.ContainsKey(obj)) return _nodes[obj];
            var node = new ObjectNode(obj);
            _nodes.Add(obj, node);
            return node;
        }
    }
}
