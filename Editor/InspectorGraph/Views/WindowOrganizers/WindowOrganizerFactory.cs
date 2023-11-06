// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System;

namespace GiantParticle.InspectorGraph.Views
{
    internal class WindowOrganizerFactory
    {
        public IWindowOrganizer GetWindowOrganizer(WindowOrganizerType type)
        {
            switch (type)
            {
                case WindowOrganizerType.TopDown:
                    return new TopDownWindowOrganizer();
                case WindowOrganizerType.BottomUp:
                    return new BottomUpWindowOrganizer();
                default:
                    throw new NotImplementedException($"Unhandled Window Organizer Type [{type}]");
            }
        }
    }
}
