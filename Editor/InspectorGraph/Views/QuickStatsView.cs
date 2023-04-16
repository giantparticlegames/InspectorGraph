// ********************************
// (C) 2022 - Giant Particle Games
// All rights reserved.
// ********************************

using System.Collections.Generic;
using System.Text;
using GiantParticle.InspectorGraph.Editor.Data.Nodes;
using UnityEngine.UIElements;

namespace GiantParticle.InspectorGraph.Editor.Views
{
    internal class QuickStatsView : VisualElement
    {
        private IObjectNode Node { get; }

        public QuickStatsView(IObjectNode node)
        {
            Node = node;
            CreateUI();
        }

        private void CreateUI()
        {
            this.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
            AddObjectType();
            AddReferencedByLabel();
            AddReferencesLabel();
        }

        private Label GetOrCreateLabel(string labelName)
        {
            var label = this.Q<Label>(labelName);
            if (label == null)
            {
                label = new Label();
                label.name = labelName;
                this.Add(label);
            }

            return label;
        }

        private void AddObjectType()
        {
            var objectTypeLabel = GetOrCreateLabel("_objectType");
            objectTypeLabel.text = $"{Node.Target.GetType().Name}";
            objectTypeLabel.tooltip = $"Object Type: {Node.Target.GetType().FullName}";
        }

        private void AddReferencesLabel()
        {
            int refCount = 0;
            int totalReferences = 0;
            foreach (IObjectNodeReference reference in Node.References)
            {
                ++refCount;
                totalReferences += reference.RefCount;
            }

            if (refCount <= 0) return;

            StringBuilder builder = new();
            builder.Append($"Refs: {refCount}");

            string tooltipText = null;
            if (totalReferences != refCount)
            {
                builder.Append($" [Total: {totalReferences}]");
                tooltipText = "* Object is referencing an object more than once";
            }

            var statsLabel = GetOrCreateLabel("_refsLabel");
            statsLabel.text = builder.ToString();
            if (tooltipText != null) statsLabel.tooltip = tooltipText;
        }

        private void AddReferencedByLabel()
        {
            var root = GlobalApplicationContext.Instance.Get<IObjectNode>();
            HashSet<IObjectNode> visitedNodes = new();
            Queue<IObjectNode> queue = new();
            queue.Enqueue(root);

            int refCount = 0;
            int totalReferences = 0;
            while (queue.Count > 0)
            {
                IObjectNode node = queue.Dequeue();
                if (visitedNodes.Contains(node)) continue;
                visitedNodes.Add(node);

                foreach (IObjectNodeReference reference in node.References)
                {
                    queue.Enqueue(reference.TargetNode);
                    if (reference.TargetNode == Node)
                    {
                        ++refCount;
                        totalReferences += reference.RefCount;
                    }
                }
            }

            if (refCount <= 0) return;

            StringBuilder builder = new();
            builder.Append($"Refs By: {refCount}");

            string tooltipText = null;
            if (totalReferences != refCount)
            {
                builder.Append($" [Total: {totalReferences}]");
                tooltipText = "* Object is being referenced multiple times by one or more objects";
            }

            var statsLabel = GetOrCreateLabel("_refByLabel");
            statsLabel.text = builder.ToString();
            if (tooltipText != null) statsLabel.tooltip = tooltipText;
        }

        public void UpdateView()
        {
            AddReferencedByLabel();
            AddReferencesLabel();
        }
    }
}
