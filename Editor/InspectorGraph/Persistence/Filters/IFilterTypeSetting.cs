// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

namespace GiantParticle.InspectorGraph.Persistence
{
    internal interface IFilterTypeSetting
    {
        string FullyQualifiedName { get; }
        bool ShowType { get; }
        bool ExpandType { get; }
    }
}
