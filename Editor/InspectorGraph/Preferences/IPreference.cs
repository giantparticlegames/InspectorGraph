// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System;

namespace GiantParticle.InspectorGraph.Editor.Preferences
{
    internal interface IPreference
    {
        string Key { get; }
        Type DataType { get; }
        void LoadData(object data);
        object Data { get; }
    }
}
