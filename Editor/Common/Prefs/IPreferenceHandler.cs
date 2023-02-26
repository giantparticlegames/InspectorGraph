// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

namespace GiantParticle.InspectorGraph.Common.Prefs
{
    public interface IPreferenceHandler
    {
        int Count { get; }
        void LoadAllPreferences();
        T GetPreference<T>() where T : class, IPreference;
        void SaveAll();
        void Save<T>() where T : class, IPreference;
    }
}
