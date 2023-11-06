// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using UnityEditor;
using UnityEngine;

namespace GiantParticle.InspectorGraph.Persistence
{
    [FilePath(
        relativePath: "GiantParticle/InspectorGraphUserPreferences.asset",
        location: FilePathAttribute.Location.PreferencesFolder)]
    internal class InspectorGraphUserPreferences : ScriptableSingleton<InspectorGraphUserPreferences>,
        IInspectorGraphUserPreferences
    {
        [SerializeField]
        private string _lastInspectedGUID;

        [SerializeField]
        private int _graphFactoryIndex;

        public string LastInspectedObjectGUID
        {
            get => _lastInspectedGUID;
            set => _lastInspectedGUID = value;
        }

        public int SelectedGraphFactoryIndex
        {
            get => _graphFactoryIndex;
            set => _graphFactoryIndex = value;
        }

        public void Save() => Save(true);
    }
}
