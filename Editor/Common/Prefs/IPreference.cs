// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System;

namespace GiantParticle.InspectorGraph.Common.Prefs
{
    public interface IPreference
    {
        string Key { get; }
        Type DataType { get; }
        void LoadData(object data);
    }
}
