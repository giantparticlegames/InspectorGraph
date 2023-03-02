// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using GiantParticle.InspectorGraph.Common.Prefs;

namespace GiantParticle.InspectorGraph
{
    [Serializable]
    internal class GeneralPreferencesData
    {
        public string LastInspectedObjectPath;
    }

    internal class GeneralPreferences : BasePreference<GeneralPreferencesData>
    {
        public override string Key => "General";

        public string LastInspectedObjectPath
        {
            get => CurrentData.LastInspectedObjectPath;
            set => CurrentData.LastInspectedObjectPath = value;
        }
    }
}
