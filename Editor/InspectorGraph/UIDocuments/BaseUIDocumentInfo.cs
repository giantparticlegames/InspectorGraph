// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace GiantParticle.InspectorGraph.Editor.UIDocuments
{
    internal interface IUIDocumentInfo<TEnum>
        where TEnum : Enum
    {
        TEnum Type { get; }
        VisualTreeAsset Asset { get; }
    }

    [Serializable]
    internal abstract class BaseUIDocumentInfo<TEnum> : IUIDocumentInfo<TEnum>
        where TEnum : Enum
    {
        [SerializeField]
        private TEnum _type;

        [SerializeField]
        private VisualTreeAsset _asset;

        public TEnum Type => _type;
        public VisualTreeAsset Asset => _asset;
    }
}
