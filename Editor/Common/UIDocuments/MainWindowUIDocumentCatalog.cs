// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using UnityEngine;

namespace GiantParticle.InspectorGraph.Editor.Common
{
    internal enum MainWindowUIDocumentType
    {
        MainWindow = 1,
        MainWindowToolbar = 100
    }

    [Serializable]
    internal class MainWindowUIDocumentInfo : BaseUIDocumentInfo<MainWindowUIDocumentType>
    {
    }

    [CreateAssetMenu(
        fileName = "MainWindowUIDocumentCatalog",
        menuName = "Giant Particle/Inspector Graph/UI Document Catalogs/Create Main Window Catalog")]
    internal class MainWindowUIDocumentCatalog :
        BaseUIDocumentCatalog<MainWindowUIDocumentType, MainWindowUIDocumentInfo>
    {
        public static IUIDocumentCatalog<MainWindowUIDocumentType> GetCatalog() =>
            GetCatalog<MainWindowUIDocumentCatalog>();
    }
}
