// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using GiantParticle.InspectorGraph.CustomAttributes;
using GiantParticle.InspectorGraph.Editor.InspectorGraph.Data.Graph;
using GiantParticle.InspectorGraph.UIToolkit;
using UnityEngine.UIElements;

namespace GiantParticle.InspectorGraph
{
    internal partial class InspectorGraphFooter : VisualElement
    {
        public event Action<float> ZoomLevelChanged;

        public InspectorGraphFooter()
        {
            LoadLayout();
            _resetZoomButton.RegisterCallback<ClickEvent>(ResetZoomButton_OnClickEvent);
            _zoomSlider.value = 1;
            _zoomSlider.RegisterValueChangedCallback(ZoomSlider_OnValueChangedEvent);
            _generalProgressBar.visible = false;
            var controller = GlobalApplicationContext.Instance.Get<IGraphController>();
            controller.SelectedFactoryChanged += UpdateDisplayMode;
            UpdateDisplayMode(controller);
        }

        private void LoadLayout()
        {
            var asset = UIToolkitHelper.LocateViewForType(this);
            if (asset == null) return;
            asset.CloneTree(this);
            UIToolkitHelper.ResolveVisualElements(this, this);
        }

        private void ResetZoomButton_OnClickEvent(ClickEvent evt)
        {
            _zoomSlider.value = 1f;
        }

        private void ZoomSlider_OnValueChangedEvent(ChangeEvent<float> zoomEvent)
        {
            var scale = zoomEvent.newValue;
            _zoomSlider.label = $"Zoom x{scale:F2}";
            ZoomLevelChanged?.Invoke(scale);
        }

        public void UpdateProgress(string label, float progress)
        {
            if (progress <= 0 || progress >= 1)
            {
                _generalProgressBar.visible = false;
                return;
            }

            _generalProgressBar.visible = true;
            _generalProgressBar.value = progress;
            _generalProgressBar.title = label;
        }

        private void UpdateDisplayMode(IGraphController controller)
        {
            var displayName = (EditorDisplayNameAttribute)Attribute.GetCustomAttribute(
                element: controller.ActiveFactory.GetType(),
                attributeType: typeof(EditorDisplayNameAttribute));
            _currentModeLabel.text = $"Displaying: {displayName.DisplayName}";
        }
    }
}
