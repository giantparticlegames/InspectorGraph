// ********************************
// (C) 2022 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using UnityEngine.UIElements;

namespace GiantParticle.InspectorGraph.Editor.ContentView
{
    internal abstract class BaseWindowContent : VisualElement, IDisposable
    {
        public event Action ContentChanged;
        public abstract void Dispose();

        protected void RaiseContentChanged()
        {
            ContentChanged?.Invoke();
        }
    }
}
