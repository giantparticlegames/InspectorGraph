// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using System.Collections.Generic;
using GiantParticle.InspectorGraph.Data;
using GiantParticle.InspectorGraph.Data.Nodes;
using GiantParticle.InspectorGraph.Editor.InspectorGraph.Data.Graph;
using UnityEngine;

namespace GiantParticle.InspectorGraph.Views
{
    internal abstract class BaseWindowOrganizer : IWindowOrganizer
    {
        protected const int kPositionXOffset = 80;
        protected const int kPositionYOffset = 30;

        protected class WindowLevelData
        {
            public readonly IObjectNode Node;
            public readonly int Level;
            public readonly InspectorWindow Window;

            public WindowLevelData(IObjectNode node, int level, InspectorWindow window)
            {
                Node = node;
                Level = level;
                Window = window;
            }
        }

        public void ReorderWindows()
        {
            HashSet<InspectorWindow> windowsVisited = new();
            IReadOnlyList<float> maxWidthPerLevel = GetMaxWidthPerLevel();
            float currentY = 0;
            int currentLevel = 0;
            ProcessWindows((windowData =>
            {
                int windowLevel = windowData.Level;
                if (currentLevel != windowLevel)
                {
                    currentY = 0;
                    currentLevel = windowLevel;
                }

                var window = windowData.Window;
                if (window == null) return;
                // Avoid reposition already repositioned window
                if (windowsVisited.Contains(window)) return;

                // Avoid moving manually moved window
                if (!window.Node.WindowData.HasBeenManuallyMoved)
                {
                    float newPositionX = GetXPosition(windowData, maxWidthPerLevel);

                    window.transform.position = new Vector3(
                        x: newPositionX,
                        y: currentY,
                        z: 0);
                }

                currentY += window.contentRect.height + kPositionYOffset;
                windowsVisited.Add(window);
            }));
        }

        protected abstract float GetXPosition(WindowLevelData windowLevelData, IReadOnlyList<float> maxWidthPerLevel);

        protected void ProcessWindows(Action<WindowLevelData> windowMethod)
        {
            var graphController = GlobalApplicationContext.Instance.Get<IGraphController>();
            var viewRegistry = GlobalApplicationContext.Instance.Get<IContentViewRegistry>();

            Queue<WindowLevelData> queue = new();
            HashSet<IObjectNode> visitedNodes = new();

            queue.Enqueue(new WindowLevelData(
                node: graphController.ActiveGraph,
                level: 0,
                window: viewRegistry.WindowByTarget(graphController.ActiveGraph.Object)));
            while (queue.Count > 0)
            {
                var item = queue.Dequeue();
                visitedNodes.Add(item.Node);

                // Add children
                foreach (IObjectReference nodeReference in item.Node.References)
                {
                    if (visitedNodes.Contains(nodeReference.TargetNode)) continue;

                    visitedNodes.Add(nodeReference.TargetNode);
                    queue.Enqueue(new WindowLevelData(
                        node: nodeReference.TargetNode,
                        level: item.Level + 1,
                        window: viewRegistry.WindowByTarget(nodeReference.TargetNode.Object)));
                }

                windowMethod?.Invoke(item);
            }
        }

        protected IReadOnlyList<float> GetMaxWidthPerLevel()
        {
            List<float> maxWidths = new();

            // Get max Width per level
            ProcessWindows(windowData =>
            {
                int windowLevel = windowData.Level;
                var window = windowData.Window;
                // Store max width
                if (maxWidths.Count > windowLevel)
                    maxWidths[windowLevel] = Math.Max(maxWidths[windowLevel], window.contentRect.width);
                else
                    maxWidths.Add(window.contentRect.width);
            });

            return maxWidths;
        }
    }
}
