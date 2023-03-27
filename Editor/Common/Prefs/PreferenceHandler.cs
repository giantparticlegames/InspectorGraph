// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using System.Collections.Generic;
using GiantParticle.InspectorGraph.Editor.Common;
using UnityEditor;
using UnityEngine;

namespace GiantParticle.InspectorGraph.Common.Prefs
{
    internal class PreferenceHandler : IPreferenceHandler
    {
        public const string kEditorPrefsSuffix = "GiantParticle-InspectorGraph";
        private Dictionary<Type, IPreference> _preferences = new();

        public int Count => _preferences.Count;

        public void LoadAllPreferences()
        {
            Type[] prefTypes = ReflectionHelper.GetAllInterfaceImplementationsCurrentAssembly(typeof(IPreference));
            for (int i = 0; i < prefTypes.Length; ++i)
            {
                IPreference pref = LoadPreferences(prefTypes[i]);
                _preferences.Add(prefTypes[i], pref);
            }
        }

        public T GetPreference<T>()
            where T : class, IPreference
        {
            return (T)_preferences[typeof(T)];
        }

        public void SaveAll()
        {
            foreach (IPreference preference in _preferences.Values)
                Save(preference);
        }

        public void Save<T>()
            where T : class, IPreference
        {
            IPreference pref = _preferences[typeof(T)];
            Save(pref);
        }

        private static IPreference LoadPreferences(Type prefType)
        {
            IPreference prefInstance = (IPreference)Activator.CreateInstance(prefType);
            var rawPrefs = EditorPrefs.GetString(GenerateKey(prefInstance));

            // Load empty data if nothing was loaded
            if (string.IsNullOrEmpty(rawPrefs))
            {
                prefInstance.LoadData(Activator.CreateInstance(prefInstance.DataType));
                return prefInstance;
            }

            // Load retrieved data
            prefInstance.LoadData(JsonUtility.FromJson(rawPrefs, prefInstance.DataType));
            return prefInstance;
        }

        private static void Save(IPreference preference)
        {
            string key = GenerateKey(preference);
            string data = JsonUtility.ToJson(preference.Data);
            EditorPrefs.SetString(key, data);
        }

        private static string GenerateKey(IPreference preference)
        {
            return $"{kEditorPrefsSuffix}-{preference.Key}";
        }
    }
}
