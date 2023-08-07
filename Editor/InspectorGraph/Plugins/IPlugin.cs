// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using GiantParticle.InspectorGraph.Editor.Preferences;

namespace GiantParticle.InspectorGraph.Editor.Plugins
{
    /// <summary>
    /// Base Plugin interface
    /// </summary>
    internal interface IPlugin : IDisposable
    {
        void ConfigurePreferences(IPreferenceHandler handler);
    }
}
