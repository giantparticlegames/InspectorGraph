// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using System.Collections.Generic;
using GiantParticle.InspectorGraph.Persistence;

namespace GiantParticle.InspectorGraph.Data.Graph.Filters
{
    internal interface ITypeFilterHandler
    {
        bool EnableFilters { get; set; }
        IReadOnlyCollection<ITypeFilter> Filters { get; }
        bool ShouldExpandObject(object objectInstance);
        bool ShouldShowObject(object objectInstance);
    }

    internal class TypeFilterHandler : ITypeFilterHandler
    {
        private readonly Dictionary<Type, ITypeFilter> _filters = new();

        public bool EnableFilters { get; set; }
        public IReadOnlyCollection<ITypeFilter> Filters => _filters.Values;

        public TypeFilterHandler(IFilterSettings settings)
        {
            EnableFilters = settings.EnableFilters;
            if (settings.TypeFilters == null || settings.TypeFilters.Count <= 0)
                return;
            for (int i = 0; i < settings.TypeFilters.Count; ++i)
            {
                var filter = new TypeFilterByName(settings.TypeFilters[i]);
                if (filter.TargetType == null) continue;

                AssignFilter(filter);
            }
        }

        private void AssignFilter(ITypeFilter filter)
        {
            // Assign filter or override existing
            if (!_filters.ContainsKey(filter.TargetType))
            {
                _filters.Add(filter.TargetType, filter);
                return;
            }

            _filters[filter.TargetType] = filter;
        }

        public bool ShouldExpandObject(object objectInstance)
        {
            if (!EnableFilters) return true;

            foreach (ITypeFilter filter in _filters.Values)
            {
                if (!filter.TargetType.IsInstanceOfType(objectInstance))
                    continue;
                return filter.ShouldExpandType;
            }

            return true;
        }

        public bool ShouldShowObject(object objectInstance)
        {
            if (!EnableFilters) return true;

            foreach (ITypeFilter filter in _filters.Values)
            {
                if (!filter.TargetType.IsInstanceOfType(objectInstance))
                    continue;
                return filter.ShouldShowType;
            }

            return true;
        }
    }
}
