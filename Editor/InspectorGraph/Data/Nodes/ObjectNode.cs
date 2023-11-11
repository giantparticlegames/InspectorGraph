// ********************************
// (C) 2022 - Giant Particle Games
// All rights reserved.
// ********************************

using System.Collections.Generic;
using UnityEngine;

namespace GiantParticle.InspectorGraph.Data.Nodes
{
    internal class ObjectNode : IObjectNode
    {
        public Object Object { get; }
        public IWindowData WindowData { get; }

        public IReadOnlyList<IObjectReference> References => _references;

        private List<IObjectReference> _references = new();

        public ObjectNode(Object target)
        {
            Object = target;
            WindowData = new WindowData(target);
        }

        public static void CreateReference(
            ObjectNode sourceObject,
            ObjectNode targetObject,
            ReferenceType referenceType)
        {
            // Reference To
            var referenceTo = new ObjectReference(
                targetNode: targetObject,
                referenceType: referenceType,
                direction: ReferenceDirection.ReferenceTo);
            sourceObject._references.Add(referenceTo);

            // Reference By
            var referenceBy = new ObjectReference(
                targetNode: sourceObject,
                referenceType: referenceType,
                direction: ReferenceDirection.ReferenceBy);
            targetObject._references.Add(referenceBy);
        }
    }
}
