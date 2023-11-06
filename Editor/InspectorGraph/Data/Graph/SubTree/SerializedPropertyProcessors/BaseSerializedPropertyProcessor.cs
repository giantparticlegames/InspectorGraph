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
        public Queue<ObjectNode> NodeQueue { get; set; }

        protected ITypeFilterHandler FilterHandler => GlobalApplicationContext.Instance.Get<ITypeFilterHandler>();

        protected IObjectNodeFactory NodeFactory => GlobalApplicationContext.Instance.Get<IObjectNodeFactory>();

        public abstract bool ProcessSerializedProperty(SerializedProperty property, ObjectNode parentNode,
            ICollection<Object> excludeSet);
    }
}
