// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using UnityEngine;

namespace GiantParticle.InspectorGraph.Data.Nodes
{
    internal interface IObjectNodeFactory
    {
        void ClearRegistry();
        ObjectNode CreateNode(Object obj, bool skipPath = false);
    }
}
