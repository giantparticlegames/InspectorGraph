// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System.Collections.Generic;
using GiantParticle.InspectorGraph.Editor.Data.Nodes.Filters;
using UnityEditor;
using UnityEngine;

namespace GiantParticle.InspectorGraph.Editor.Data.Nodes.SerializedPropertyProcessors
{
    internal abstract class BaseSerializedPropertyProcessor : ISerializedPropertyProcessor
    {
        public abstract int Priority { get; }
        public Queue<ObjectNode> NodeQueue { get; set; }
        public ITypeFilterHandler FilterHandler { get; set; }

        public abstract bool ProcessSerializedProperty(SerializedProperty property, ObjectNode parentNode,
            ICollection<Object> excludeSet);
    }
}
