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

                    InspectorWindow directionalSource = sourceWindow;
                    InspectorWindow directionalTarget = targetWindow;
                    if (nodeReference.Direction == ReferenceDirection.ReferenceBy)
                    {
                        directionalSource = targetWindow;
                        directionalTarget = sourceWindow;
                    }

                    var connectionLine = viewRegistry.GetConnection(
                        source: directionalSource,
                        dest: directionalTarget,
                        refType: nodeReference.RefType);
                    if (connectionLine != null)
                    {
                        connectionLine.Data.ReferenceCount += 1;
                    }
                    else
                    {
                        var line = new ConnectionLine(
                            source: directionalSource,
                            dest: directionalTarget,
                            refType: nodeReference.RefType);
                        container.Add(line);
                        line.SendToBack();
                        // Register line
                        viewRegistry.RegisterConnection(line);
                    }

                    queue.Enqueue(nodeReference.TargetNode);
                }
            }

            // Organize connections
            viewRegistry.ExecuteOnEachWindow(window =>
            {
                int fromTotal = viewRegistry.ConnectionsFromWindowCount(window);
                if (fromTotal > 1)
                {
                    int count = 1;
                    foreach (ConnectionLine line in viewRegistry.ConnectionsFromWindow(window))
                    {
                        line.Data.SourceCount = count;
                        line.Data.SourceTotal = fromTotal;
                        ++count;
                    }
                }

                int toTotal = viewRegistry.ConnectionsToWindowCount(window);
                if (toTotal > 0)
                {
                    int count = 1;
                    foreach (ConnectionLine line in viewRegistry.ConnectionsToWindow(window))
                    {
                        line.Data.DestinationCount = count;
                        line.Data.DestinationTotal = toTotal;
                        ++count;
                    }
                }
            });
        }
    }
}
