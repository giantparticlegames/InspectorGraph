// ********************************
// (C) 2022 - Giant Particle Games
// All rights reserved.
// ********************************

using UnityEngine;
using UnityEngine.UIElements;

namespace GiantParticle.InspectorGraph.Editor.Manipulators
{
    /// <summary>
    /// Button id.
    /// <a href="https://docs.unity3d.com/ScriptReference/UIElements.PointerEventBase_1-button.html">See PointerEventBase doc</a>
    /// </summary>
    internal enum ManipulatorButton
    {
        Left = 0,
        Right = 1,
        Middle = 2
    }

    internal struct ActivatorCombination
    {
        public ManipulatorButton MouseButton;
        public EventModifiers Modifiers;

        public ActivatorCombination(ManipulatorButton mouseButton)
        {
            MouseButton = mouseButton;
            Modifiers = EventModifiers.None;
        }

        public ActivatorCombination(ManipulatorButton mouseButton, EventModifiers modifiers)
        {
            MouseButton = mouseButton;
            Modifiers = modifiers;
        }

        public bool ShouldActivate(PointerDownEvent evt)
        {
            if (Modifiers != EventModifiers.None && Modifiers != evt.modifiers) return false;
            if (MouseButton == (ManipulatorButton)evt.button) return true;
            return false;
        }
    }

    internal abstract class BaseDragManipulator : PointerManipulator, IScalableManipulator
    {
        private Vector3 _startClickPosition;
        private Vector3 _deltaClickPosition;
        private bool _enabled;
        private ActivatorCombination[] _activators;

        protected Vector3 StartPosition => _startClickPosition;

        protected Vector3 PositionDelta => new Vector3(
            x: _deltaClickPosition.x * MovementScale.x,
            y: _deltaClickPosition.y * MovementScale.y,
            z: _deltaClickPosition.z * MovementScale.z);

        private Vector3 _movementScale = Vector3.one;

        public Vector3 MovementScale
        {
            get => _movementScale;
            set => _movementScale = value;
        }

        protected BaseDragManipulator(VisualElement handle, ActivatorCombination[] activators = null)
        {
            this.target = handle;
            _activators = activators ?? new[] { new ActivatorCombination(ManipulatorButton.Left) };
        }

        protected override void RegisterCallbacksOnTarget()
        {
            // Click and hold
            target.RegisterCallback<PointerDownEvent>(OnPointerDownEvent);
            target.RegisterCallback<PointerMoveEvent>(OnPointerMoveEvent);
            target.RegisterCallback<PointerUpEvent>(OnPointerUpEvent);
            target.RegisterCallback<PointerCaptureOutEvent>(OnPointerCaptureOutEvent);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            // Click and hold
            target.UnregisterCallback<PointerDownEvent>(OnPointerDownEvent);
            target.UnregisterCallback<PointerMoveEvent>(OnPointerMoveEvent);
            target.UnregisterCallback<PointerUpEvent>(OnPointerUpEvent);
            target.UnregisterCallback<PointerCaptureOutEvent>(OnPointerCaptureOutEvent);
        }

        private void OnPointerDownEvent(PointerDownEvent evt)
        {
            if (_enabled) return;
            if (target.HasPointerCapture(evt.pointerId)) return;
            bool activate = false;
            for (int i = 0; i < _activators.Length; ++i)
            {
                if (_activators[i].ShouldActivate(evt))
                {
                    activate = true;
                    break;
                }
            }

            if (!activate) return;

            _startClickPosition = evt.position;
            target.CapturePointer(evt.pointerId);
            _enabled = true;
            OnPointerDown(evt);
        }


        private void OnPointerMoveEvent(PointerMoveEvent evt)
        {
            if (!_enabled) return;
            if (!target.HasPointerCapture(evt.pointerId)) return;

            _deltaClickPosition = evt.position - _startClickPosition;
            OnPointerMove(evt);
        }


        private void OnPointerUpEvent(PointerUpEvent evt)
        {
            if (!_enabled) return;
            if (!target.HasPointerCapture(evt.pointerId)) return;

            target.ReleasePointer(evt.pointerId);
        }

        private void OnPointerCaptureOutEvent(PointerCaptureOutEvent evt)
        {
            if (!_enabled) return;
            _enabled = false;
        }

        // Abstract methods
        protected abstract void OnPointerDown(PointerDownEvent evt);
        protected abstract void OnPointerMove(PointerMoveEvent evt);
    }
}
