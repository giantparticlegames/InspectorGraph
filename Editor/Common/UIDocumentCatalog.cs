// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GiantParticle.InspectorGraph.Editor.Common
{
    internal enum UIDocumentTypes
    {
        MainWindow = 1,
        InspectorWindow = 2,
        FilterTypeSettings = 3,
        Settings = 4,
        TypeOptions = 5
    }

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

    internal interface IUIDocumentCatalog
    {
        IUIDocumentInfo this[UIDocumentTypes type] { get; }
    }

    // [CreateAssetMenu(
    //     fileName = "UIDocumentCatalog",
    //     menuName = "GiantParticle/Inspector Graph/Create UI Document Catalog")]
    internal class UIDocumentCatalog : ScriptableObject, IUIDocumentCatalog
    {
        [SerializeField]
        private UIDocumentInfo[] _entries;

        [NonSerialized]
        private Dictionary<UIDocumentTypes, UIDocumentInfo> _index;

        public IUIDocumentInfo this[UIDocumentTypes type]
        {
            get
            {
                EnsureIndex();
                if (!_index.ContainsKey(type))
                {
                    Debug.LogError($"[{typeof(UIDocumentCatalog)}] instance does not contain type [{type}]");
                    return null;
                }

                return _index[type];
            }
        }

        private void EnsureIndex()
        {
            if (_index != null) return;
            _index = new Dictionary<UIDocumentTypes, UIDocumentInfo>();

            if (_entries == null) return;
            for (int i = 0; i < _entries.Length; ++i)
            {
                var entry = _entries[i];
                if (_index.ContainsKey(entry.Type))
                {
                    Debug.LogWarning($"Entry with type [{entry.Type}] already exist");
                    continue;
                }

                _index.Add(entry.Type, entry);
            }
        }

        public static IUIDocumentCatalog GetCatalog()
        {
            var guids = AssetDatabase.FindAssets($"t:{typeof(UIDocumentCatalog).FullName}");
            if (guids == null || guids.Length <= 0)
                throw new FileNotFoundException(
                    $"Instance of Scriptable Object [{typeof(UIDocumentCatalog).Name}] not found in project");

            // Return the first available asset
            return AssetDatabase.LoadAssetAtPath<UIDocumentCatalog>(AssetDatabase.GUIDToAssetPath(guids[0]));
        }
    }
}
