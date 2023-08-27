// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System;

namespace GiantParticle.InspectorGraph.CustomAttributes
{
    public class EditorDisplayPriorityAttribute : Attribute
    {
        public int Priority { get; }

        public EditorDisplayPriorityAttribute(int priority)
        {
            Priority = priority;
        }
    }
}
