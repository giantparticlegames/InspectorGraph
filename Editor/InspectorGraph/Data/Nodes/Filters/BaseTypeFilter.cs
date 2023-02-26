// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using GiantParticle.InspectorGraph.Common.Prefs;
using GiantParticle.InspectorGraph.Editor.Common;

namespace GiantParticle.InspectorGraph.Editor.Data.Nodes.Filters
{
    [Serializable]
    public class TypeFilterData
    {
        public bool ShouldExpandType;
        public bool ShouldShowType;
    }

    public abstract class TypeFilterPreference<T> : BasePreference<TypeFilterData>
    {
        public override string Key => $"TypeFilter-{typeof(T).FullName}";

        public bool ShouldExpandType
        {
            get => CurrentData.ShouldExpandType;
            set => CurrentData.ShouldExpandType = value;
        }

        public bool ShouldShowType
        {
            get => CurrentData.ShouldShowType;
            set => CurrentData.ShouldShowType = value;
        }
    }

    public abstract class BaseTypeFilter<T, PT> : ITypeFilter
        where PT : TypeFilterPreference<T>, new()
    {
        public Type TargetType => typeof(T);
        public string MenuName => TargetType.Name;

        public bool ShouldExpandType
        {
            get => CachedPreference.ShouldExpandType;
            set
            {
                CachedPreference.ShouldExpandType = value;
                // Save
                IPreferenceHandler handler = GlobalApplicationContext.Instance.Get<IPreferenceHandler>();
                handler.Save<PT>();
            }
        }

        public bool ShouldShowType
        {
            get => CachedPreference.ShouldShowType;
            set
            {
                CachedPreference.ShouldShowType = value;
                // Save
                IPreferenceHandler handler = GlobalApplicationContext.Instance.Get<IPreferenceHandler>();
                handler.Save<PT>();
            }
        }

        private PT _cachedPreference;

        private PT CachedPreference
        {
            get
            {
                if (_cachedPreference == null)
                {
                    IPreferenceHandler handler = GlobalApplicationContext.Instance.Get<IPreferenceHandler>();
                    _cachedPreference = handler.GetPreference<PT>();
                }

                return _cachedPreference;
            }
        }
    }
}
