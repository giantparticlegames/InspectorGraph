// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using UnityEditor;

namespace GiantParticle.InspectorGraph.Settings
{
    internal static class InspectorGraphSettingsRegister
    {
        public const string kMenuPath = "Giant Particle/Inspector Graph";

        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            var provider = new InspectorGraphSettingsProvider(kMenuPath, SettingsScope.Project);
            return provider;
        }
    }
}
