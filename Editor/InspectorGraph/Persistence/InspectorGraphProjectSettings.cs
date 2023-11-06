// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using UnityEditor;
using UnityEngine;

namespace GiantParticle.InspectorGraph.Persistence
{
    [FilePath(
        relativePath: "ProjectSettings/GiantParticle/InspectorGraphSettings.asset",
        location: FilePathAttribute.Location.ProjectFolder)]
    internal class InspectorGraphProjectSettings : ScriptableSingleton<InspectorGraphProjectSettings>,
        IInspectorGraphProjectSettings
    {
        [SerializeField]
        private InspectorWindowSettings _windowSettings = new InspectorWindowSettings();

        [SerializeField]
        private FilterSettings _filterSettings = new FilterSettings();

        [SerializeField]
        private ConnectionSettings _connectionSettings = new ConnectionSettings();

        public string[] SerializedFieldNames => new string[]
        {
            nameof(_windowSettings), nameof(_filterSettings), nameof(_connectionSettings),
        };

        public IInspectorWindowSettings WindowSettings => _windowSettings;

        public IFilterSettings FilterSettings => _filterSettings;

        public IConnectionSettings ConnectionSettings => _connectionSettings;

        public void Save() => Save(true);
    }
}
