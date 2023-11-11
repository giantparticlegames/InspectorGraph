// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using UnityEngine.UIElements;

namespace GiantParticle.InspectorGraph.Plugins
{
    /// <summary>
    /// Interface to allow for a Plugin to configure it's view
    /// </summary>
    internal interface IInspectorGraphPluginView
    {
        void ConfigureView(VisualElement root);
    }
}
