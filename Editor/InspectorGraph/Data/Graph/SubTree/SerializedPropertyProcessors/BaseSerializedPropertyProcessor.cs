// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System.Collections.Generic;
using GiantParticle.InspectorGraph.Data.Nodes;
using GiantParticle.InspectorGraph.Data.Graph.Filters;
using UnityEditor;
using UnityEngine;

namespace GiantParticle.InspectorGraph.Data.Graph.SubTree.SerializedPropertyProcessors
{
    internal abstract class BaseSerializedPropertyProcessor : ISerializedPropertyProcessor
    {
        // TODO: Move this to an Attribute
        public abstract int Priority { get; }
        public Queue<ObjectNode> NodeQueue { get; set; }

        public ITypeFilterHandler FilterHandler
        {
            get => GlobalApplicationContext.Instance.Get<ITypeFilterHandler>();
        }

        public abstract bool ProcessSerializedProperty(SerializedProperty property, ObjectNode parentNode,
            ICollection<Object> excludeSet);
    }
}
