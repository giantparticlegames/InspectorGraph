// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System.Collections.Generic;
using GiantParticle.InspectorGraph.Data.Nodes;
using UnityEditor;
using UnityEngine;

namespace GiantParticle.InspectorGraph.Data.Graph.SubTree.SerializedPropertyProcessors
{
    internal interface ISerializedPropertyProcessor
    {
        Queue<ObjectNode> NodeQueue { get; set; }

        bool ProcessSerializedProperty(SerializedProperty property, ObjectNode parentNode,
            ICollection<Object> excludeSet);
    }
}
