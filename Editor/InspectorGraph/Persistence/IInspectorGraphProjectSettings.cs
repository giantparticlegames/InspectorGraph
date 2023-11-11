// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

namespace GiantParticle.InspectorGraph.Persistence
{
    internal interface IInspectorGraphProjectSettings
    {
        IInspectorWindowSettings WindowSettings { get; }

        IFilterSettings FilterSettings { get; }
        IConnectionSettings ConnectionSettings { get; }

        string[] SerializedFieldNames { get; }
    }
}
