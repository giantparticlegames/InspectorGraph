// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using UnityEngine;

namespace GiantParticle.InspectorGraph.Editor.Data.Nodes.Filters
{
    internal class GameObjectTypeFilterPreference : TypeFilterPreference<GameObject>
    {
    }

    internal class GameObjectTypeFilter : BaseTypeFilter<GameObject, GameObjectTypeFilterPreference>
    {
    }
}
