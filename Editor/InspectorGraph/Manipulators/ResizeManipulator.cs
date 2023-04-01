// ********************************
// (C) 2022 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace GiantParticle.InspectorGraph.Editor.Manipulators
{
    internal class ResizeManipulator : BaseDragManipulator
    {
        public event Action TargetResized;
        private Vector2 _startingSize;
        private VisualElement _resizeTarget;
        private bool _enabled;

        public ResizeManipulator(VisualElement handle, VisualElement resizeTarget) : base(handle)
        {
            _resizeTarget = resizeTarget;
        }

        protected override void OnPointerDown(PointerDownEvent evt)
        {
            _startingSize = new Vector2(
                x: _resizeTarget.contentRect.width,
                y: _resizeTarget.contentRect.height);
        }

        protected override void OnPointerMove(PointerMoveEvent evt)
        {
            _resizeTarget.style.width = new StyleLength(_startingSize.x + PositionDelta.x);
            _resizeTarget.style.height = new StyleLength(_startingSize.y + PositionDelta.y);
            // Remove limits
            if (_resizeTarget.style.maxWidth != new StyleLength(StyleKeyword.None))
                _resizeTarget.style.maxWidth = new StyleLength(StyleKeyword.None);
            if (_resizeTarget.style.maxHeight != new StyleLength(StyleKeyword.None))
                _resizeTarget.style.maxHeight = new StyleLength(StyleKeyword.None);

            TargetResized?.Invoke();
        }
    }
}
