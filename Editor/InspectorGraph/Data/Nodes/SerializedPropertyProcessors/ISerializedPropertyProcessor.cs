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
    internal interface ISerializedPropertyProcessor
    {
        int Priority { get; }
        Queue<ObjectNode> NodeQueue { get; set; }
        ITypeFilterHandler FilterHandler { get; set; }

        bool ProcessSerializedProperty(SerializedProperty property, ObjectNode parentNode,
            ICollection<Object> excludeSet);
    }
}
