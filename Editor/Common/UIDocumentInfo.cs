// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace GiantParticle.InspectorGraph.Editor.Common
{
    internal interface IUIDocumentInfo
    {
        UIDocumentTypes Type { get; }
        VisualTreeAsset Asset { get; }
    }

    [Serializable]
    internal class UIDocumentInfo : IUIDocumentInfo
    {
        [SerializeField]
        private UIDocumentTypes _type;

        [SerializeField]
        private VisualTreeAsset _asset;

        public UIDocumentTypes Type => _type;
        public VisualTreeAsset Asset => _asset;
    }
}
