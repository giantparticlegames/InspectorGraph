// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using UnityEngine.UIElements;

namespace GiantParticle.InspectorGraph.Editor.Plugins
{
    /// <summary>
    /// Interface to allow for a Plugin to configure its view
    /// </summary>
    internal interface IConfigurableView
    {
        void ConfigureView(VisualElement root);
    }
}
