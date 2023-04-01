// ********************************
// (C) 2022 - Giant Particle Games
// All rights reserved.
// ********************************

using UnityEngine;

namespace GiantParticle.InspectorGraph.Editor.Manipulators
{
    internal interface IScalableManipulator
    {
        Vector3 MovementScale { get; set; }
    }
}
