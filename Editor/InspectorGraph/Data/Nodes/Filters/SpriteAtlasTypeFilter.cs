#if PACKAGE_UNITY_2D_SPRITE
// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using UnityEngine.U2D;

namespace GiantParticle.InspectorGraph.Editor.Data.Nodes.Filters
{
    internal class SpriteAtlasTypeFilterPreference : TypeFilterPreference<SpriteAtlas>
    {
    }

    internal class SpriteAtlasTypeFilter : BaseTypeFilter<SpriteAtlas, SpriteAtlasTypeFilterPreference>
    {
    }
}
#endif
