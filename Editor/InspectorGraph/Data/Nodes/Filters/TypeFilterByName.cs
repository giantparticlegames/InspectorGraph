// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using GiantParticle.InspectorGraph.Editor.Common;
using GiantParticle.InspectorGraph.Settings;

namespace GiantParticle.InspectorGraph.Editor.Data.Nodes.Filters
{
    internal class TypeFilterByName : ITypeFilter
    {
        public Type TargetType { get; }

        public bool ShouldExpandType { get; set; }

        public bool ShouldShowType { get; set; }

        public TypeFilterByName(FilterTypeSettings filterTypeSettings)
        {
            TargetType = ReflectionHelper.GetTypeByName(filterTypeSettings.FullyQualifiedName);
            ShouldShowType = filterTypeSettings.ShowType;
            ShouldExpandType = filterTypeSettings.ExpandType;
        }
    }
}
