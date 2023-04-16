// ********************************
// (C) 2022 - Giant Particle Games
// All rights reserved.
// ********************************

using UnityEngine;
using UnityEngine.UIElements;

namespace GiantParticle.InspectorGraph.Editor
{
    internal static class VisualElementExtensions
    {
        public static void ResizeToFit<T>(this VisualElement container)
        {
            float width = 0;
            float height = 0;
            for (int i = 0; i < container.childCount; ++i)
            {
                var item = container[i];
                if (!(item is T)) continue;
                width = Mathf.Max(width, item.transform.position.x + item.contentRect.width);
                height = Mathf.Max(height, item.transform.position.y + item.contentRect.height);
            }

            container.style.minWidth = new StyleLength(width);
            container.style.minHeight = new StyleLength(height);
        }
    }
}
