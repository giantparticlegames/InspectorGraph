// ********************************
// (C) 2022 - Giant Particle Games 
// All rights reserved.
// ********************************

using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace GiantParticle.InspectorGraph.Editor.Common.Manipulators
{
    public class DragManipulator : BaseDragManipulator, IPositionManipulator
    {
        public event Action<VisualElement> PositionChanged;
        private Vector3 _startTargetPosition;
        private VisualElement _dragTarget;
        private bool _enabled;

        public DragManipulator(VisualElement handle, VisualElement target) : base(handle)
        {
            _dragTarget = target;
        }

        protected override void OnPointerDown(PointerDownEvent evt)
        {
            _startTargetPosition = _dragTarget.transform.position;
        }

        protected override void OnPointerMove(PointerMoveEvent evt)
        {
            _dragTarget.transform.position = new Vector2(
                Mathf.Clamp(_startTargetPosition.x + PositionDelta.x, 0, float.MaxValue),
                Mathf.Clamp(_startTargetPosition.y + PositionDelta.y, 0, float.MaxValue));

            PositionChanged?.Invoke(_dragTarget);
        }
    }
}
