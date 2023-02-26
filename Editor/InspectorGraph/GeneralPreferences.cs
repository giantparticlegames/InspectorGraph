// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using GiantParticle.InspectorGraph.Common.Prefs;

namespace GiantParticle.InspectorGraph
{
    [Serializable]
    public class GeneralPreferencesData
    {
        public string LastInspectedObjectPath;
    }

    public class GeneralPreferences : BasePreference<GeneralPreferencesData>
    {
        public override string Key => "General";

        public string LastInspectedObjectPath
        {
            get => CurrentData.LastInspectedObjectPath;
            set { CurrentData.LastInspectedObjectPath = value; }
        }
    }
}
