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
        public string LastInspectedObjectGUID;
    }

    internal class GeneralPreferences : BasePreference<GeneralPreferencesData>
    {
        public override string Key => "General";

        public string LastInspectedObjectGUID
        {
            get => CurrentData.LastInspectedObjectGUID;
            set => CurrentData.LastInspectedObjectGUID = value;
        }
    }
}
