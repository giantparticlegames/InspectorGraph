// ********************************
// (C) 2022 - Giant Particle Games 
// All rights reserved.
// ********************************

using UnityEngine;

namespace GiantParticle.InspectorGraph.ContentView
{
    public enum ContentViewMode
    {
        InspectorElement,
        IMGUI,
        Preview,
        StaticPreview
    }

    public static class ContentViewModeExtensions
    {
        public static bool IsObjectCompatible(this ContentViewMode mode, Object obj)
        {
            if (mode == ContentViewMode.InspectorElement && obj is GameObject) return false;
            if (mode == ContentViewMode.Preview)
            {
                var tmpEditor = UnityEditor.Editor.CreateEditor(obj);
                bool isCompatible = tmpEditor.HasPreviewGUI();
                UnityEditor.Editor.DestroyImmediate(tmpEditor);
                return isCompatible;
            }

            return true;
        }
    }
}
