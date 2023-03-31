// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using UnityEngine;

namespace GiantParticle.InspectorGraph.Editor.Common
{
    internal enum InspectorWindowUIDocumentType
    {
        InspectorWindow = 1
    }

    [Serializable]
    internal class InspectorWindowUIDocumentInfo : BaseUIDocumentInfo<InspectorWindowUIDocumentType>
    {
    }

    [CreateAssetMenu(
        fileName = "InspectorWindowUIDocumentCatalog",
        menuName = "Giant Particle/Inspector Graph/UI Document Catalogs/Create Inspector Window Catalog")]
    internal class InspectorWindowUIDocumentCatalog :
        BaseUIDocumentCatalog<InspectorWindowUIDocumentType, InspectorWindowUIDocumentInfo>
    {
        public static IUIDocumentCatalog<InspectorWindowUIDocumentType> GetCatalog() =>
            GetCatalog<InspectorWindowUIDocumentCatalog>();
    }
}
