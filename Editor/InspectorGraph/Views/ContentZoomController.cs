// ********************************
// (C) 2022 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using GiantParticle.InspectorGraph.Editor.Data;
using GiantParticle.InspectorGraph.Editor.Manipulators;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GiantParticle.InspectorGraph.Editor
{
    internal class ContentZoomController : VisualElement
    {
        public event Action<VisualElement> ZoomLevelChanged;
        private ToolbarButton _resetButton;
        private Slider _slider;
        private readonly VisualElement _zoomTarget;

        public ContentZoomController(VisualElement zoomTarget)
        {
            _zoomTarget = zoomTarget;
            CreateUI();
        }

        private void CreateUI()
        {
            this.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
            // Add button
            _resetButton = new ToolbarButton(ResetZoom);
            _resetButton.text = "Reset";
            this.Add(_resetButton);

            // Slider
            _slider = new Slider(0.1f, 2f, SliderDirection.Horizontal);
            _slider.value = 1f;
            _slider.label = $"Zoom x{1f:F2}";
            _slider.RegisterValueChangedCallback(OnZoomLevelChanged);
            this.Add(_slider);
        }

        private void ResetZoom()
        {
            _slider.value = 1f;
        }

        private void OnZoomLevelChanged(ChangeEvent<float> zoomEvent)
        {
            var scale = zoomEvent.newValue;
            _slider.label = $"Zoom x{scale:F2}";
            // Update window manipulators
            var registry = GlobalApplicationContext.Instance.Get<IContentViewRegistry>();
            registry.ExecuteOnEachWindow((window) =>
            {
                for (int i = 0; i < window.Manipulators.Count; ++i)
                {
                    var manipulator = window.Manipulators[i];
                    if (manipulator is IScalableManipulator scalableManipulator)
                        scalableManipulator.MovementScale = new Vector3(1f / scale, 1f / scale, 1);
                }
            });

            _zoomTarget.transform.scale = new Vector3(scale, scale, 1);
            ZoomLevelChanged?.Invoke(_zoomTarget);
        }
    }
}
