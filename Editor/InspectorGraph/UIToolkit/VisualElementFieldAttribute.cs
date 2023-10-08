// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System;

namespace GiantParticle.InspectorGraph.UIToolkit
{
    [AttributeUsage(
        validOn: AttributeTargets.Field,
        AllowMultiple = true,
        Inherited = true)]
    internal class VisualElementFieldAttribute : Attribute
    {
        public string ElementID { get; }

        public VisualElementFieldAttribute()
        {
        }

        public VisualElementFieldAttribute(string id)
        {
            ElementID = id;
        }
    }
}
