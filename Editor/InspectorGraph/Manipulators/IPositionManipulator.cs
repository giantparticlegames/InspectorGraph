// ********************************
// (C) 2022 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using UnityEngine.UIElements;

namespace GiantParticle.InspectorGraph.Editor.Manipulators
{
    internal interface IPositionManipulator
    {
        event Action<VisualElement> PositionChanged;
    }
}
