// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

namespace GiantParticle.InspectorGraph.Persistence
{
    internal interface IInspectorGraphUserPreferences
    {
        string LastInspectedObjectGUID { get; set; }
        int SelectedGraphFactoryIndex { get; set; }

        void Save();
    }
}
