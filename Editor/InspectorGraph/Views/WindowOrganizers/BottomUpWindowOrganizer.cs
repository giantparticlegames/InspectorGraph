// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System.Collections.Generic;

namespace GiantParticle.InspectorGraph.Views
{
    internal class BottomUpWindowOrganizer : BaseWindowOrganizer
    {
        protected override float GetXPosition(WindowLevelData windowLevelData, IReadOnlyList<float> maxWidthPerLevel)
        {
            int windowLevel = windowLevelData.Level;
            float newPositionX = 0;
            for (int i = maxWidthPerLevel.Count - 1; i > windowLevel; --i)
                newPositionX += maxWidthPerLevel[i] + kPositionXOffset;

            return newPositionX;
        }
    }
}
