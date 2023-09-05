// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System.Collections.Generic;
using UnityEngine;

namespace GiantParticle.InspectorGraph.Data.Nodes
{
    internal class ObjectNodeFactory : IObjectNodeFactory
    {
        private Dictionary<Object, ObjectNode> _nodes = new();

        public void ClearRegistry()
        {
            _nodes.Clear();
        }

        public ObjectNode CreateNode(Object obj)
        {
            if (_nodes.ContainsKey(obj)) return _nodes[obj];
            var node = new ObjectNode(obj);
            _nodes.Add(obj, node);
            return node;
        }
    }
}
