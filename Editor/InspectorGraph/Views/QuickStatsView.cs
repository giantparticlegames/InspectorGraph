// ********************************
// (C) 2022 - Giant Particle Games
// All rights reserved.
// ********************************

using System.Collections.Generic;
using System.Text;
using GiantParticle.InspectorGraph.Data.Nodes;
using GiantParticle.InspectorGraph.Data.Stats;
using UnityEngine;
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

        private Label GetOrCreateLabel(string labelName, bool addDivision = false)
        {
            var label = this.Q<Label>(labelName);
            if (label == null)
            {
                label = new Label();
                label.name = labelName;
                if (addDivision)
                {
                    label.style.borderLeftColor = new StyleColor(new Color(1, 1, 1, 0.5f));
                    label.style.borderLeftWidth = new StyleFloat(1);
                }

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
            ReferenceStats stats = ObjectNodeStats.GetReferenceStats(Node, ReferenceDirection.ReferenceTo);
            if (stats.TotalReferences <= 0) return;

            var statsLabel = GetOrCreateLabel("_refsLabel", true);
            // Label text
            StringBuilder builder = new StringBuilder();
            builder.Append($"Refs: {stats.TotalReferences}");
            if (stats.TotalReferences != stats.UniqueReferences)
                builder.Append($" [Unique {stats.UniqueReferences}]");
            statsLabel.text = builder.ToString();

            // Tooltip Text
            builder.Clear();
            builder.AppendLine("Number of References:");
            builder.AppendLine($"- Total: {stats.TotalReferences}");
            builder.AppendLine($"- Unique: {stats.UniqueReferences}");
            int totalStats = stats.ReferenceTypes;
            int statCount = 0;
            builder.AppendLine("References By Type:");
            foreach (ReferenceByTypeStats refStats in stats.StatsByType)
            {
                builder.Append($"- {refStats.ReferenceType}: {refStats.TotalReferences}");
                if (refStats.TotalUniqueReference != refStats.TotalReferences)
                    builder.Append($" [Unique: {refStats.TotalUniqueReference}]");

                ++statCount;
                if (statCount < totalStats) builder.AppendLine("");
            }

            statsLabel.tooltip = builder.ToString();
        }

        private void AddReferencedByLabel()
        {
            ReferenceStats stats = ObjectNodeStats.GetReferenceStats(Node, ReferenceDirection.ReferenceBy);
            if (stats.TotalReferences <= 0) return;

            var statsLabel = GetOrCreateLabel("_refsByLabel", true);
            // Label text
            StringBuilder builder = new StringBuilder();
            builder.Append($"Refs By: {stats.TotalReferences}");
            if (stats.TotalReferences != stats.UniqueReferences)
                builder.Append($" [Unique {stats.UniqueReferences}]");
            statsLabel.text = builder.ToString();

            // Tooltip Text
            builder.Clear();
            builder.AppendLine("Number of References:");
            builder.AppendLine($"- Total: {stats.TotalReferences}");
            builder.AppendLine($"- Unique: {stats.UniqueReferences}");
            int totalStats = stats.ReferenceTypes;
            int statCount = 0;
            builder.AppendLine("References By Type:");
            foreach (ReferenceByTypeStats refStats in stats.StatsByType)
            {
                builder.Append($"- {refStats.ReferenceType}: {refStats.TotalReferences}");
                if (refStats.TotalUniqueReference != refStats.TotalReferences)
                    builder.Append($" [Unique: {refStats.TotalUniqueReference}]");

                ++statCount;
                if (statCount < totalStats) builder.AppendLine("");
            }

            statsLabel.tooltip = builder.ToString();
        }

        public void UpdateView()
        {
            AddReferencedByLabel();
            AddReferencesLabel();
        }
    }
}
