// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using System.Collections.Generic;
using GiantParticle.InspectorGraph.Editor.Common;

namespace GiantParticle.InspectorGraph.Editor.Data.Nodes.Filters
{
    public class TypeFilterHandler
    {
        private readonly Dictionary<Type, ITypeFilter> _filters = new();

        public IReadOnlyCollection<ITypeFilter> Filters => _filters.Values;

        public TypeFilterHandler()
        {
            Type[] allImplementations = ReflectionHelper.GetAllInterfaceImplementations(typeof(ITypeFilter));
            for (int i = 0; i < allImplementations.Length; ++i)
                AssignFilter((ITypeFilter)Activator.CreateInstance(allImplementations[i]));
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
