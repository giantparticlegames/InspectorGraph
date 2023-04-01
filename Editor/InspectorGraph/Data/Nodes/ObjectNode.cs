// ********************************
// (C) 2022 - Giant Particle Games
// All rights reserved.
// ********************************

using System.Collections.Generic;
using UnityEngine;

namespace GiantParticle.InspectorGraph.Editor.Data.Nodes
{
    internal interface IObjectNode
    {
        Object Target { get; }
        IWindowData WindowData { get; }
        IEnumerable<IObjectNodeReference> References { get; }
    }

    internal class ObjectNode : IObjectNode
    {
        public Object Target { get; }
        public IWindowData WindowData { get; }

        public IEnumerable<IObjectNodeReference> References
        {
            get
            {
                foreach (var referencesValue in _references.Values)
                {
                    foreach (var value in referencesValue.Values)
                    {
                        yield return value;
                    }
                }
            }
        }

        private Dictionary<Object, Dictionary<ReferenceType, ObjectNodeReference>> _references = new();

        public ObjectNode(IWindowData data)
        {
            Target = data.Target;
            WindowData = data;
        }

        public void AddNode(ObjectNode objectNode, ReferenceType refType)
        {
            var key = objectNode.Target;
            if (_references.ContainsKey(key) && _references[key].ContainsKey(refType))
            {
                _references[key][refType].RefCount++;
                return;
            }

            _references.Add(key,
                new Dictionary<ReferenceType, ObjectNodeReference>()
                {
                    { refType, new ObjectNodeReference(objectNode, refType) }
                });
        }
    }
}
