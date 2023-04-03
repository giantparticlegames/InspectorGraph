// ********************************
// (C) 2022 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using System.Collections.Generic;
using GiantParticle.InspectorGraph.Editor.Data;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GiantParticle.InspectorGraph.Editor.ContentView
{
    internal static class WindowContentFactory
    {
        private static Dictionary<Type, ContentViewMode> _cachedModes = new();

        public static ContentViewMode PreferredViewModeForObject(Object target)
        {
            var targetType = target.GetType();
            if (_cachedModes.ContainsKey(targetType)) return _cachedModes[targetType];

            // Create Inspector so we can test capabilities
            var inspector = UnityEditor.Editor.CreateEditor(target);

            // Prefer Preview over everything else
            if (IsPreviewCompatible(inspector))
                return ReturnMode(targetType, ContentViewMode.Preview, inspector);

            // Create inspector
            bool isUIToolkitCompatible = inspector.CreateInspectorGUI() != null;
            if (isUIToolkitCompatible)
                return ReturnMode(targetType, ContentViewMode.InspectorElement, inspector);

            // At this point we just guess the best fit
            if (target is ScriptableObject)
                return ReturnMode(targetType, ContentViewMode.IMGUI, inspector);

            // Default to Static Preview
            return ReturnMode(targetType, ContentViewMode.StaticPreview, inspector);
        }

        private static bool IsPreviewCompatible(UnityEditor.Editor editor)
        {
            // Hack for AnimationClipEditor. If not checked, the `HasPreviewGUI` function will
            // throw an exception
            if (string.Equals(editor.GetType().Name, "AnimationClipEditor")) return true;
            return editor.HasPreviewGUI();
        }

        private static ContentViewMode ReturnMode(Type objectType, ContentViewMode mode,
            UnityEditor.Editor inspector = null)
        {
            // Destroy inspector
            if (inspector != null) UnityEditor.Editor.DestroyImmediate(inspector);
            // Cache default view mode only if is not a Prefab.
            // Prefabs may or may not have preview.
            if (objectType != typeof(GameObject)) _cachedModes.Add(objectType, mode);
            return mode;
        }

        public static BaseWindowContent CreateContent(ContentViewMode mode, IWindowData windowData,
            bool forceMini = false)
        {
            switch (mode)
            {
                case ContentViewMode.InspectorElement:
                    return new InspectorElementWindowContent(windowData);
                case ContentViewMode.IMGUI:
                    return new IMGUIWindowContent(windowData);
                case ContentViewMode.Preview:
                    return new IMGUIPreviewWindowContent(windowData);
                case ContentViewMode.StaticPreview:
                    return new StaticPreviewWindowContent(windowData, forceMini);
            }

            throw new NotImplementedException($"Unhandled Content View Mode [{mode}]");
        }
    }
}
