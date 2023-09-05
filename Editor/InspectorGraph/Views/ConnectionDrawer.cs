// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System.Collections.Generic;
using GiantParticle.InspectorGraph.Data;
using GiantParticle.InspectorGraph.Data.Nodes;
using GiantParticle.InspectorGraph.Editor.InspectorGraph.Data.Graph;
using UnityEngine.UIElements;

namespace GiantParticle.InspectorGraph.Views
{
    internal class ConnectionDrawer : IConnectionDrawer
    {
        public void DrawConnections(VisualElement container)
        {
            Queue<IObjectNode> queue = new();
            HashSet<IObjectNode> visitedNodes = new();

            var graphController = GlobalApplicationContext.Instance.Get<IGraphController>();
            var viewRegistry = GlobalApplicationContext.Instance.Get<IContentViewRegistry>();
            queue.Enqueue(graphController.ActiveGraph);

            while (queue.Count > 0)
            {
                var node = queue.Dequeue();
                if (visitedNodes.Contains(node)) continue;
                visitedNodes.Add(node);

                var sourceWindow = viewRegistry.WindowByTarget(node.Object);
                foreach (IObjectReference nodeReference in node.References)
                {
                    if (nodeReference.Direction != graphController.ActiveFactory.GraphDirection) continue;

                    var targetWindow = viewRegistry.WindowByTarget(nodeReference.TargetNode.Object);
                    if (targetWindow == null) continue;
                    if (!viewRegistry.ContainsConnection(sourceWindow, targetWindow))
                    {
                        var sourcePoint = nodeReference.Direction == ReferenceDirection.ReferenceTo
                            ? sourceWindow
                            : targetWindow;
                        var targetPoint = nodeReference.Direction == ReferenceDirection.ReferenceTo
                            ? targetWindow
                            : sourceWindow;
                        var line = new ConnectionLine(
                            source: sourcePoint,
                            dest: targetPoint,
                            refType: nodeReference.RefType);
                        container.Add(line);
                        line.SendToBack();
                        // Register line
                        viewRegistry.RegisterConnection(line);
                    }

                    queue.Enqueue(nodeReference.TargetNode);
                }
            }
        }
    }
}
