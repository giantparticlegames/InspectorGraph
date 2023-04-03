// ********************************
// (C) 2022 - Giant Particle Games
// All rights reserved.
// ********************************

using UnityEngine;

namespace GiantParticle.InspectorGraph.Editor.ContentView
{
    internal enum ContentViewMode
    {
        InspectorElement,
        IMGUI,
        Preview,
        StaticPreview
    }

    internal static class ContentViewModeExtensions
    {
        public static bool IsObjectCompatible(this ContentViewMode mode, Object obj)
        {
            if (mode == ContentViewMode.InspectorElement && obj is GameObject) return false;
            if (mode == ContentViewMode.Preview)
            {
                var tmpEditor = UnityEditor.Editor.CreateEditor(obj);
                bool isCompatible = false;
                // Hack for AnimationClipEditor. If not checked, the `HasPreviewGUI` function will
                // throw an exception
                if (string.Equals(tmpEditor.GetType().Name, "AnimationClipEditor")) isCompatible = true;
                else isCompatible = tmpEditor.HasPreviewGUI();

                UnityEditor.Editor.DestroyImmediate(tmpEditor);
                return isCompatible;
            }

            return true;
        }
    }
}
