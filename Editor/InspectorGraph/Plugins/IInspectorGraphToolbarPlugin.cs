// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using UnityEngine.UIElements;

namespace GiantParticle.InspectorGraph.Editor.Plugins
{
    /// <summary>
    /// Interface for Plugins that will go on the main Toolbar
    /// </summary>
    internal interface IInspectorGraphToolbarPlugin : IPlugin, IConfigurableView
    {
    }
}
