// ****************
// Giant Particle Inc.
// All rights reserved.
// ****************

using UnityEngine.UIElements;

namespace GiantParticle.InspectorGraph.Editor.Common.Manipulators
{
    public class BringToFrontManipulator : PointerManipulator
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
