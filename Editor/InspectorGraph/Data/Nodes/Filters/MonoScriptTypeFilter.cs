// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using UnityEditor;

namespace GiantParticle.InspectorGraph.Editor.Data.Nodes.Filters
{
    internal class MonoScriptTypeFilterPreference : TypeFilterPreference<MonoScript>
    {
    }

    internal class MonoScriptTypeFilter : BaseTypeFilter<MonoScript, MonoScriptTypeFilterPreference>
    {
    }
}
