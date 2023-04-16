// ********************************
// (C) 2022 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using System.Collections.Generic;

namespace GiantParticle.InspectorGraph.Editor
{
    internal interface IApplicationContext
    {
        bool Contains<T>() where T : class;
        T Get<T>() where T : class;
        void Add<T>(object instance) where T : class;
        void Remove<T>() where T : class;
    }

    internal class ApplicationContext : IApplicationContext
    {
        private Dictionary<Type, object> _objectsByType = new();

        public bool Contains<T>() where T : class
        {
            return _objectsByType.ContainsKey(typeof(T));
        }

        public T Get<T>()
            where T : class
        {
            Type targetType = typeof(T);
            if (!_objectsByType.ContainsKey(targetType)) return null;

            return _objectsByType[targetType] as T;
        }

        public void Add<T>(object instance)
            where T : class
        {
            Type targetType = typeof(T);
            _objectsByType.Add(targetType, instance);
        }

        public void Remove<T>()
            where T : class
        {
            Type targetType = typeof(T);
            if (_objectsByType.ContainsKey(targetType))
                _objectsByType.Remove(targetType);
        }
    }

    internal static class GlobalApplicationContext
    {
        private static IApplicationContext _instance;

        public static bool IsInstantiated
        {
            get => _instance != null;
        }

        public static IApplicationContext Instance
        {
            get => _instance;
        }

        public static void Instantiate()
        {
            if (_instance != null) return;
            _instance = new ApplicationContext();
        }

        public static void Dispose()
        {
            _instance = null;
        }
    }
}
