// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using UnityEngine;

namespace GiantParticle.InspectorGraph.Editor.UIDocuments
{
    internal enum InspectorWindowUIDocumentType
    {
        InspectorWindow = 1
    }

    [Serializable]
    internal class InspectorWindowUIDocumentInfo : BaseUIDocumentInfo<InspectorWindowUIDocumentType>
    {
    }

#if GIANT_PARTICLE_DEVELOPMENT
    [CreateAssetMenu(
        fileName = "InspectorWindowUIDocumentCatalog",
        menuName = "Giant Particle/Inspector Graph/UI Document Catalogs/Create Inspector Window Catalog")]
#endif
    internal class InspectorWindowUIDocumentCatalog :
        BaseUIDocumentCatalog<InspectorWindowUIDocumentType, InspectorWindowUIDocumentInfo>
    {
        public static IUIDocumentCatalog<InspectorWindowUIDocumentType> GetCatalog() =>
            GetCatalog<InspectorWindowUIDocumentCatalog>();
    }
}
