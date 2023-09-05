// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System;

namespace GiantParticle.InspectorGraph.Data.Nodes
{
    [Flags]
    internal enum ReferenceDirection
    {
        ReferenceTo = 0b0001,
        ReferenceBy = 0b0010
    }
}
