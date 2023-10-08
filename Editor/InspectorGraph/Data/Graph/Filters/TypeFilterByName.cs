// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using GiantParticle.InspectorGraph.Persistence;

namespace GiantParticle.InspectorGraph.Data.Graph.Filters
{
    internal class TypeFilterByName : ITypeFilter
    {
        public Type TargetType { get; }

        public bool ShouldExpandType { get; set; }

        public bool ShouldShowType { get; set; }

        public TypeFilterByName(FilterTypeSetting filterTypeSettings)
        {
            TargetType = ReflectionHelper.GetTypeByName(filterTypeSettings.FullyQualifiedName);
            ShouldShowType = filterTypeSettings.ShowType;
            ShouldExpandType = filterTypeSettings.ExpandType;
        }
    }
}
