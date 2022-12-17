// ****************
// Giant Particle Inc.
// All rights reserved.
// ****************

using System;
using UnityEngine.UIElements;

namespace GiantParticle.InspectorGraph.Editor.Common.Manipulators
{
    public interface IPositionManipulator
    {
        event Action<VisualElement> PositionChanged;
    }
}
