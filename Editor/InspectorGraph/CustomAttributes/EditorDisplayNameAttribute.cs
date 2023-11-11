// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System;

namespace GiantParticle.InspectorGraph.CustomAttributes
{
    public class EditorDisplayNameAttribute : Attribute
    {
        public string DisplayName { get; }

        public EditorDisplayNameAttribute(string name)
        {
            DisplayName = name;
        }
    }
}
