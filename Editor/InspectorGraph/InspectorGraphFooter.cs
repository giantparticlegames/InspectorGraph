// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using GiantParticle.InspectorGraph.UIDocuments;
using UnityEngine.UIElements;

namespace GiantParticle.InspectorGraph
{
    internal partial class InspectorGraphFooter : VisualElement
    {
        public event Action<float> ZoomLevelChanged;

        public InspectorGraphFooter()
        {
            var catalog = GlobalApplicationContext.Instance.Get<IUIDocumentCatalog<MainWindowUIDocumentType>>();
            IUIDocumentInfo<MainWindowUIDocumentType> info = catalog[MainWindowUIDocumentType.MainWindowFooter];
            info.Asset.CloneTree(this);
            AssignVisualElements();

            _resetZoomButton.RegisterCallback<ClickEvent>(ResetZoomButton_OnClickEvent);
            _zoomSlider.value = 1;
            _zoomSlider.RegisterValueChangedCallback(ZoomSlider_OnValueChangedEvent);
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
    }
}
