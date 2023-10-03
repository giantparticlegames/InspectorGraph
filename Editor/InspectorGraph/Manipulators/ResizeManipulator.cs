// ********************************
// (C) 2022 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace GiantParticle.InspectorGraph.Manipulators
{
    internal class ResizeManipulator : BaseDragManipulator
    {
        public event Action TargetResized;
        private Vector2 _startingSize;
        private VisualElement _resizeTarget;
        private bool _enabled;
        private Vector2 _minValues;

        public ResizeManipulator(VisualElement handle, VisualElement resizeTarget, Vector2 minValues) :
            base(handle)
        {
            _resizeTarget = resizeTarget;
            _minValues = minValues;
        }

        protected override void OnPointerDown(PointerDownEvent evt)
        {
            _startingSize = new Vector2(
                x: _resizeTarget.contentRect.width,
                y: _resizeTarget.contentRect.height);
        }

        protected override void OnPointerMove(PointerMoveEvent evt)
        {
            _resizeTarget.style.width = new StyleLength(
                Mathf.Max(
                    a: _minValues.x,
                    b: _startingSize.x + PositionDelta.x));
            _resizeTarget.style.height = new StyleLength(
                Mathf.Max(
                    a: _minValues.y,
                    b: _startingSize.y + PositionDelta.y));
            // Remove limits
            if (_resizeTarget.style.maxWidth != new StyleLength(StyleKeyword.None))
                _resizeTarget.style.maxWidth = new StyleLength(StyleKeyword.None);
            if (_resizeTarget.style.maxHeight != new StyleLength(StyleKeyword.None))
                _resizeTarget.style.maxHeight = new StyleLength(StyleKeyword.None);

            TargetResized?.Invoke();
        }
    }
}
