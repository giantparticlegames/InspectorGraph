// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System.Collections.Generic;
using GiantParticle.InspectorGraph.Data.Nodes;

namespace GiantParticle.InspectorGraph.Persistence
{
    internal interface IConnectionSettings
    {
        List<ReferenceColorSettings> ColorSettings { get; }

        ReferenceColorSettings GetColorSettings(ReferenceType referenceType);
    }
}
