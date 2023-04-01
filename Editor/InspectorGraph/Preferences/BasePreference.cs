// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System;

namespace GiantParticle.InspectorGraph.Editor.Preferences
{
    internal abstract class BasePreference<T> : IPreference
    {
        public abstract string Key { get; }
        public Type DataType => typeof(T);
        public object Data => CurrentData;

        protected T CurrentData { get; private set; }

        public void LoadData(object data)
        {
            CurrentData = (T)data;
        }
    }
}
