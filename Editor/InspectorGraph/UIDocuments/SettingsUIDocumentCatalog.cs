// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using UnityEngine;

namespace GiantParticle.InspectorGraph.Editor.UIDocuments
{
    internal enum SettingsUIDocumentType
    {
        SettingsPanel = 1,
        ReferenceColorSettings = 100,
        FilterTypeSettings = 200,
        FilterTypeOptions = 201
    }

    [Serializable]
    internal class SettingsUIDocumentInfo : BaseUIDocumentInfo<SettingsUIDocumentType>
    {
    }

    [CreateAssetMenu(
        fileName = "SettingsUIDocumentCatalog",
        menuName = "Giant Particle/Inspector Graph/UI Document Catalogs/Create Settings Catalog")]
    internal class SettingsUIDocumentCatalog :
        BaseUIDocumentCatalog<SettingsUIDocumentType, SettingsUIDocumentInfo>
    {
        public static IUIDocumentCatalog<SettingsUIDocumentType> GetCatalog() =>
            GetCatalog<SettingsUIDocumentCatalog>();
    }
}
