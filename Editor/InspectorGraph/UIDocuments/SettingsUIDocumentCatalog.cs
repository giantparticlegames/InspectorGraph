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
        FilterTypeOptions = 201,
        WindowSizeSettings = 300
    }

    [Serializable]
    internal class SettingsUIDocumentInfo : BaseUIDocumentInfo<SettingsUIDocumentType>
    {
    }

#if GIANT_PARTICLE_DEVELOPMENT
    [CreateAssetMenu(
        fileName = "SettingsUIDocumentCatalog",
        menuName = "Giant Particle/Inspector Graph/UI Document Catalogs/Create Settings Catalog")]
#endif
    internal class SettingsUIDocumentCatalog :
        BaseUIDocumentCatalog<SettingsUIDocumentType, SettingsUIDocumentInfo>
    {
        public static IUIDocumentCatalog<SettingsUIDocumentType> GetCatalog() =>
            GetCatalog<SettingsUIDocumentCatalog>();
    }
}
