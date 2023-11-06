// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System;

namespace GiantParticle.InspectorGraph.CustomAttributes
{
    public class InternalPriorityAttribute : Attribute
    {
        public int Priority { get; }

        public InternalPriorityAttribute(int priority)
        {
            Priority = priority;
        }
    }
}
