// ********************************
// (C) 2022 - Giant Particle Games
// All rights reserved.
// ********************************

using UnityEngine.UIElements;

namespace GiantParticle.InspectorGraph.Editor.Manipulators
{
    internal abstract class BaseHoverManipulator : PointerManipulator
    {
        protected BaseHoverManipulator(VisualElement handle)
        {
            this.target = handle;
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<PointerEnterEvent>(OnPointerEnterEvent);
            target.RegisterCallback<PointerLeaveEvent>(OnPointerLeaveEvent);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<PointerEnterEvent>(OnPointerEnterEvent);
            target.UnregisterCallback<PointerLeaveEvent>(OnPointerLeaveEvent);
        }

        private void OnPointerEnterEvent(PointerEnterEvent evt)
        {
            OnHover(evt);
        }

        private void OnPointerLeaveEvent(PointerLeaveEvent evt)
        {
            OnExit(evt);
        }

        // Abstract Methods
        protected abstract void OnHover(PointerEnterEvent evt);
        protected abstract void OnExit(PointerLeaveEvent evt);
    }
}
