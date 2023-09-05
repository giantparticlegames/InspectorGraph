// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using System.Collections.Generic;
using GiantParticle.InspectorGraph.Data;
using GiantParticle.InspectorGraph.Data.Nodes;
using GiantParticle.InspectorGraph.Editor.InspectorGraph.Data.Graph;
using GiantParticle.InspectorGraph.Views;
using UnityEngine;

namespace GiantParticle.InspectorGraph.Editor.InspectorGraph.Views
{
    internal class TopDownWindowOrganizer : IWindowOrganizer
    {
        private const int kPositionXOffset = 80;
        private const int kPositionYOffset = 30;

        public void ReorderWindows()
        {
            Queue<Tuple<IObjectNode, int>> queue = new();
            HashSet<InspectorWindow> windowsVisited = new();
            HashSet<IObjectNode> visitedNodes = new();
            var graphController = GlobalApplicationContext.Instance.Get<IGraphController>();
            var viewRegistry = GlobalApplicationContext.Instance.Get<IContentViewRegistry>();

            List<float> maxWidthPerLevel = new List<float>();
            float currentY = 0;
            int currentLevel = 0;

            queue.Enqueue(new Tuple<IObjectNode, int>(graphController.ActiveGraph, 0));
            while (queue.Count > 0)
            {
                var item = queue.Dequeue();
                IObjectNode node = item.Item1;
                int level = item.Item2;

                // Add children
                foreach (IObjectReference nodeReference in node.References)
                {
                    if (visitedNodes.Contains(nodeReference.TargetNode)) continue;

                    visitedNodes.Add(nodeReference.TargetNode);
                    queue.Enqueue(new Tuple<IObjectNode, int>(nodeReference.TargetNode, level + 1));
                }

                if (currentLevel != level)
                {
                    currentY = 0;
                    currentLevel = level;
                }

                InspectorWindow window = viewRegistry.WindowByTarget(item.Item1.Object);
                if (window == null) continue;
                // Avoid reposition already repositioned window
                if (windowsVisited.Contains(window)) continue;

                // Store max width
                if (maxWidthPerLevel.Count > level)
                    maxWidthPerLevel[level] = Math.Max(maxWidthPerLevel[level], window.contentRect.width);
                else
                    maxWidthPerLevel.Add(window.contentRect.width);

                // Avoid moving manually moved window
                if (!window.Node.WindowData.HasBeenManuallyMoved)
                {
                    float newPositionX = 0;
                    for (int i = 0; i < level; ++i)
                        newPositionX += maxWidthPerLevel[i] + kPositionXOffset;

                    window.transform.position = new Vector3(
                        x: newPositionX,
                        y: currentY,
                        z: 0);
                }

                currentY += window.contentRect.height + kPositionYOffset;
                windowsVisited.Add(window);
            }
        }
    }
}
