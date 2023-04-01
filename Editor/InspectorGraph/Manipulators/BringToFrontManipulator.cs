// ********************************
// (C) 2022 - Giant Particle Games
// All rights reserved.
// ********************************

using UnityEngine.UIElements;

namespace GiantParticle.InspectorGraph.Editor.Manipulators
{
    internal class BringToFrontManipulator : PointerManipulator
    {
        public BringToFrontManipulator(VisualElement target)
        {
            this.target = target;
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<PointerDownEvent>(OnPointerDownEvent);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<PointerDownEvent>(OnPointerDownEvent);
        }

        private void OnPointerDownEvent(PointerDownEvent evt)
        {
            this.target.BringToFront();
        }
    }
}
