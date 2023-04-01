// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GiantParticle.InspectorGraph.Editor.UIDocuments
{
    internal interface IUIDocumentCatalog<TEnum>
        where TEnum : Enum
    {
        IUIDocumentInfo<TEnum> this[TEnum type] { get; }
    }

    internal abstract class BaseUIDocumentCatalog<TEnum, TInfo> : ScriptableObject, IUIDocumentCatalog<TEnum>
        where TEnum : Enum
        where TInfo : IUIDocumentInfo<TEnum>
    {
        [SerializeField]
        private TInfo[] _entries;

        [NonSerialized]
        private Dictionary<TEnum, TInfo> _index;

        public IUIDocumentInfo<TEnum> this[TEnum type]
        {
            get
            {
                EnsureIndex();
                if (!_index.ContainsKey(type))
                {
                    Debug.LogError($"[{typeof(TInfo)}] instance does not contain type [{type}]");
                    return null;
                }

                return _index[type];
            }
        }

        private void EnsureIndex()
        {
            if (_index != null) return;
            _index = new Dictionary<TEnum, TInfo>();

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

            // Validate all entries
            var enumValues = Enum.GetValues(typeof(TEnum));
            for (int i = 0; i < enumValues.Length; ++i)
            {
                TEnum entryKey = (TEnum)enumValues.GetValue(i);
                if (_index.ContainsKey(entryKey)) continue;
                Debug.LogWarning($"Missing entry in collection for value [{entryKey}]");
            }
        }

        protected static IUIDocumentCatalog<TEnum> GetCatalog<T>()
            where T : BaseUIDocumentCatalog<TEnum, TInfo>
        {
            string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).FullName}");
            if (guids == null || guids.Length <= 0)
                throw new FileNotFoundException(
                    $"Instance of Scriptable Object [{nameof(T)}] not found in project");
            // Return the first available asset
            return AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guids[0]));
        }
    }
}
