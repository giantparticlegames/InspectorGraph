// ********************************
// (C) 2022 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace GiantParticle.InspectorGraph.Editor.Common.Manipulators
{
    internal class DragManipulator : BaseDragManipulator, IPositionManipulator
    {
        public event Action<VisualElement> PositionChanged;
        private Vector3 _startTargetPosition;
        private VisualElement _dragTarget;
        private bool _enabled;

        public DragManipulator(VisualElement handle, VisualElement target,
            ManipulatorButton activatorButton = ManipulatorButton.Left) :
            base(handle, activatorButton)
        {
            _dragTarget = target;
        }

        protected override void OnPointerDown(PointerDownEvent evt)
        {
            _startTargetPosition = _dragTarget.transform.position;
        }

        protected override void OnPointerMove(PointerMoveEvent evt)
        {
            _dragTarget.transform.position = new Vector3(
                x: _startTargetPosition.x + PositionDelta.x,
                y: _startTargetPosition.y + PositionDelta.y,
                z: 0f);

            PositionChanged?.Invoke(_dragTarget);
        }
    }
}
