// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System;

namespace GiantParticle.InspectorGraph.Editor.Data.Nodes.Filters
{
    internal interface ITypeFilter
    {
        /// <summary>
        /// Indicate the target type
        /// </summary>
        Type TargetType { get; }

        /// <summary>
        /// Indicate if an object of type <see cref="TargetType"/> should be expanded in the hierarchy
        /// </summary>
        bool ShouldExpandType { get; set; }

        /// <summary>
        /// Indicate if an object of type <see cref="TargetType"/> should show in the hierarchy
        /// </summary>
        bool ShouldShowType { get; set; }
    }
}
