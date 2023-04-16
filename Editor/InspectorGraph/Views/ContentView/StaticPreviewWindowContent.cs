// ********************************
// (C) 2022 - Giant Particle Games
// All rights reserved.
// ********************************

using GiantParticle.InspectorGraph.Editor.Data;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GiantParticle.InspectorGraph.Editor.ContentView
{
    internal class StaticPreviewWindowContent : BaseWindowContent
    {
        private IWindowData _windowData;
        private VisualElement _view;

        public StaticPreviewWindowContent(IWindowData windowData, bool forceMini)
        {
            _windowData = windowData;
            windowData.CreateNewSerializedTarget();

            Texture2D preview = forceMini
                ? AssetPreview.GetMiniThumbnail(windowData.Target)
                : AssetPreview.GetAssetPreview(windowData.Target);
            bool shouldWaitForTexture = false;
            if (preview == null)
            {
                preview = AssetPreview.GetMiniThumbnail(windowData.Target);
                shouldWaitForTexture = true;
            }

            _view = new VisualElement();
            LoadPreviewTexture(preview);

            // Wait for preview texture to be loaded
            if (shouldWaitForTexture) _view.schedule.Execute(CheckForPreviewTexture);
            this.Add(_view);
            this.AddToClassList($"{ContentViewMode.StaticPreview}");
        }

        public override void Dispose()
        {
            _view.RemoveFromHierarchy();
            _view = null;
        }

        private void LoadPreviewTexture(Texture2D texture)
        {
            _view.style.backgroundImage = new StyleBackground(texture);
            _view.style.width = Mathf.Min(128, texture.width);
            _view.style.height = Mathf.Min(128, texture.height);
        }

        private void CheckForPreviewTexture()
        {
            Texture2D preview = AssetPreview.GetAssetPreview(_windowData.Target);
            if (preview == null)
            {
                _view.schedule.Execute(CheckForPreviewTexture);
                return;
            }

            LoadPreviewTexture(preview);
        }
    }
}
