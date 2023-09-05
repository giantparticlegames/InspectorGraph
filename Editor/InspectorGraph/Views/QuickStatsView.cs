// ********************************
// (C) 2022 - Giant Particle Games
// All rights reserved.
// ********************************

using System.Collections.Generic;
using System.Text;
using GiantParticle.InspectorGraph.Data.Nodes;
using UnityEngine.UIElements;

namespace GiantParticle.InspectorGraph.Views
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
            objectTypeLabel.text = $"{Node.Object.GetType().Name}";
            objectTypeLabel.tooltip = $"Object Type:\n{Node.Object.GetType().FullName}";
        }

        private void AddReferencesLabel()
        {
            int refCount = 0;
            HashSet<IObjectNode> uniqueReferences = new();
            for (int i = 0; i < Node.References.Count; ++i)
            {
                if (Node.References[i].Direction == ReferenceDirection.ReferenceTo)
                {
                    ++refCount;
                    uniqueReferences.Add(Node.References[i].TargetNode);
                }
            }

            if (refCount <= 0) return;

            StringBuilder builder = new();
            builder.Append($"Refs: {refCount}");

            string tooltipText = $"Number of References to other objects: {refCount}";
            if (uniqueReferences.Count != refCount)
            {
                builder.Append($" [Total: {uniqueReferences.Count}]");
                tooltipText = "* Object is referencing an object more than once";
            }

            var statsLabel = GetOrCreateLabel("_refsLabel");
            statsLabel.text = builder.ToString();
            statsLabel.tooltip = tooltipText;
        }

        private void AddReferencedByLabel()
        {
            int refCount = 0;
            HashSet<IObjectNode> uniqueReferences = new();

            for (int i = 0; i < Node.References.Count; ++i)
            {
                if (Node.References[i].Direction == ReferenceDirection.ReferenceBy)
                {
                    ++refCount;
                    uniqueReferences.Add(Node.References[i].TargetNode);
                }
            }

            if (refCount <= 0) return;

            StringBuilder builder = new();
            builder.Append($"Refs By: {refCount}");

            string tooltipText = $"Number of References by other Objects: {refCount}";
            if (uniqueReferences.Count != refCount)
            {
                builder.Append($" [Total: {uniqueReferences.Count}]");
                tooltipText = "* Object is being referenced multiple times by one or more objects";
            }

            var statsLabel = GetOrCreateLabel("_refByLabel");
            statsLabel.text = builder.ToString();
            statsLabel.tooltip = tooltipText;
        }

        public void UpdateView()
        {
            AddReferencedByLabel();
            AddReferencesLabel();
        }
    }
}
