// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System.Collections.Generic;

namespace GiantParticle.InspectorGraph.Persistence
{
    internal interface IFilterSettings
    {
        bool EnableFilters { get; }
        List<FilterTypeSetting> TypeFilters { get; }
    }
}
