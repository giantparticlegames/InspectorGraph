// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System;

namespace GiantParticle.InspectorGraph.Common.Prefs
{
    public abstract class BasePreference<T> : IPreference
    {
        public abstract string Key { get; }
        public Type DataType => typeof(T);

        protected T CurrentData { get; private set; }

        public void LoadData(object data)
        {
            CurrentData = (T)data;
        }
    }
}
