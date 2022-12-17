// ********************************
// (C) 2022 - Giant Particle Games 
// All rights reserved.
// ********************************

using System;
using UnityEditor;
using UnityEngine;

namespace GiantParticle.InspectorGraph
{
    [Serializable]
    public class Preferences
    {
        private const string kEditorPrefsKey = "GiantParticle-InspectorGraph-Prefs";

        public string LastInspectedObjectPath;

        public static Preferences LoadPreferences()
        {
            var rawPrefs = EditorPrefs.GetString(kEditorPrefsKey);
            if (string.IsNullOrEmpty(rawPrefs)) return new Preferences();
            return JsonUtility.FromJson<Preferences>(rawPrefs);
        }

        public void Save()
        {
            var raw = JsonUtility.ToJson(this);
            EditorPrefs.SetString(kEditorPrefsKey, raw);
        }
    }
}
