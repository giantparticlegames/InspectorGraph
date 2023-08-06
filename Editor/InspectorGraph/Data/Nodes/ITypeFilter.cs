// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using GiantParticle.InspectorGraph.Editor.Data.Nodes.Filters;

namespace GiantParticle.InspectorGraph.Editor.Data.Nodes
{
    internal interface ITypeFilter
    {
        ITypeFilterHandler FilterHandler { get; set; }
    }
}
