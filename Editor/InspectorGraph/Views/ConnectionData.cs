// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

namespace GiantParticle.InspectorGraph.Views
{
    internal class ConnectionData
    {
        public int SourceCount { get; set; }
        public int SourceTotal { get; set; }
        public int DestinationCount { get; set; }
        public int DestinationTotal { get; set; }
        public int ReferenceCount { get; set; }

        public ConnectionData()
        {
            SourceCount = 1;
            SourceTotal = 1;
            DestinationCount = 1;
            DestinationTotal = 1;
            ReferenceCount = 1;
        }
    }
}
