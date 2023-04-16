// ********************************
// (C) 2022 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace GiantParticle.InspectorGraph.Editor.Views
{
    internal class WindowStateController : IDisposable
    {
        public enum State
        {
            Normal,
            Minimized,
            Maximized
        }

        private VisualElement _window;
        private VisualElement _windowContent;
        private State _currentState;
        private State _previousState;

        private Dictionary<string, State> _buttonHandlers = new();

        // original lengths
        private StyleLength _originalHeight;
        private StyleLength _originalMaxHeight;

        public State CurrentState
        {
            get => _currentState;
            set
            {
                if (_currentState == value) return;
                ChangeState(value);
            }
        }

        public WindowStateController(VisualElement window, VisualElement windowContent)
        {
            _window = window;
            _windowContent = windowContent;
            _currentState = State.Normal;
            _previousState = State.Normal;
        }

        public void Dispose()
        {
            foreach (KeyValuePair<string, State> keyValuePair in _buttonHandlers)
                DeregisterClick(keyValuePair.Key, keyValuePair.Value);
            _buttonHandlers.Clear();
        }

        public void RegisterCase(string buttonName, State state)
        {
            if (_buttonHandlers.ContainsKey(buttonName))
                throw new Exception("Button case already being handled");
            _buttonHandlers.Add(buttonName, state);
            RegisterClick(buttonName, state);
        }

        public void ForceNormalState()
        {
            _currentState = State.Normal;
            SaveOriginalStyles();
        }

        private void RegisterClick(string buttonName, State state)
        {
            var button = _window.Q(buttonName);
            if (button == null) return;
            switch (state)
            {
                case State.Minimized:
                    button.RegisterCallback<ClickEvent>(OnMinimizeClickEvent);
                    break;
                case State.Maximized:
                    button.RegisterCallback<ClickEvent>(OnMaximizeClickEvent);
                    break;
                default:
                    throw new NotImplementedException($"Handling for state [{state}] not implemented");
            }
        }

        private void DeregisterClick(string buttonName, State state)
        {
            var button = _window.Q(buttonName);
            if (button == null) return;
            switch (state)
            {
                case State.Minimized:
                    button.UnregisterCallback<ClickEvent>(OnMinimizeClickEvent);
                    break;
                case State.Maximized:
                    button.UnregisterCallback<ClickEvent>(OnMaximizeClickEvent);
                    break;
                default:
                    throw new NotImplementedException($"Handling for state [{state}] not implemented");
            }
        }

        private void OnMinimizeClickEvent(ClickEvent evt)
        {
            ChangeState(State.Minimized);
        }

        private void OnMaximizeClickEvent(ClickEvent evt)
        {
            ChangeState(State.Maximized);
        }

        private void ChangeState(State state)
        {
            if (_currentState == state)
            {
                if (_currentState == State.Normal) return;
                // Restore previous state
                state = _previousState;
            }

            bool shouldSaveStyles = _currentState == State.Normal;

            _previousState = _currentState == State.Maximized ? _currentState : State.Normal;
            _currentState = state;
            switch (state)
            {
                case State.Normal:
                    // Restore original height
                    _window.style.height = _originalHeight;
                    _window.style.maxHeight = _originalMaxHeight;
                    // Update styles
                    _windowContent.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
                    break;
                case State.Minimized:
                    // Save original Height
                    if (shouldSaveStyles) SaveOriginalStyles();

                    // Update styles
                    _window.style.height = new StyleLength(StyleKeyword.None);
                    _windowContent.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
                    break;
                case State.Maximized:
                    // Save original Height
                    if (shouldSaveStyles) SaveOriginalStyles();

                    // Update styles
                    _window.style.height = new StyleLength(StyleKeyword.Auto);
                    if (_window.style.maxHeight != new StyleLength(StyleKeyword.None))
                        _window.style.maxHeight = new StyleLength(StyleKeyword.None);
                    _windowContent.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
                    break;
            }
        }

        private void SaveOriginalStyles()
        {
            if (_window.style.height != new StyleLength(StyleKeyword.None))
                _originalHeight = _window.style.height;
            if (_window.style.maxHeight != new StyleLength(StyleKeyword.None))
                _originalMaxHeight = _window.style.maxHeight;
        }
    }
}
