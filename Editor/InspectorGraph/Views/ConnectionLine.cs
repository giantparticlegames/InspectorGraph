// ********************************
// (C) 2022 - Giant Particle Games
// All rights reserved.
// ********************************

using GiantParticle.InspectorGraph.Data.Nodes;
using GiantParticle.InspectorGraph.Persistence;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace GiantParticle.InspectorGraph.Views
{
    internal class ConnectionLine : VisualElement
    {
        private const float kArrowLength = 10f;
        private const float kArrowWidth = 10f;

        public enum ViewMode
        {
            Normal,
            Highlighted
        }

        public ViewMode Mode
        {
            get => ForceHighlightCount > 0 ? ViewMode.Highlighted : _mode;
            set => _mode = value;
        }

        public VisualElement Source { get; }
        public VisualElement Destination { get; }
        public ReferenceType ReferenceType { get; }
        public int ForceHighlightCount { get; set; }

        public ConnectionData Data { get; }

        private ViewMode _mode;
        private IConnectionSettings _connectionSettings;
        private IReferenceColorSettings _colorSettings;

        public ConnectionLine(VisualElement source, VisualElement dest, ReferenceType refType) : base()
        {
            Source = source;
            Destination = dest;
            ReferenceType = refType;
            Data = new ConnectionData();
            var projectSettings = GlobalApplicationContext.Instance.Get<IInspectorGraphProjectSettings>();
            _connectionSettings = projectSettings.ConnectionSettings;
            _colorSettings = projectSettings.ConnectionSettings.GetColorSettings(refType);

            var line = new IMGUIContainer(DrawLine);
            this.Add(line);
        }

        private void DrawLine()
        {
            Handles.BeginGUI();
            float thickness = 1;
            Color lineColor = _colorSettings.NormalColor;
            switch (Mode)
            {
                case ViewMode.Normal:
                    lineColor = _colorSettings.NormalColor;
                    thickness = 1;
                    break;
                case ViewMode.Highlighted:
                    lineColor = _colorSettings.HighlightedColor;
                    thickness = 2;
                    break;
            }

            Vector3 startPoint = Source.transform.position +
                                 new Vector3(
                                     x: Source.contentRect.width,
                                     y: Source.contentRect.height * 0.25f
                                        + (Source.contentRect.height * 0.5f) * (Data.SourceCount * 1f) /
                                        (Data.SourceTotal * 1f + 1),
                                     z: 0);
            Vector3 endPoint = Destination.transform.position +
                               new Vector3(
                                   x: 0,
                                   y: Destination.contentRect.height * 0.25f
                                      + (Destination.contentRect.height * 0.5f) * (Data.DestinationCount * 1f) /
                                      (Data.DestinationTotal * 1f + 1),
                                   z: 0);

            // Draw Curve
            Handles.DrawBezier(startPosition: startPoint,
                endPosition: endPoint + Vector3.left * kArrowLength,
                startTangent: startPoint + Vector3.right * 25,
                endTangent: endPoint + Vector3.left * 25,
                color: lineColor,
                texture: Texture2D.whiteTexture,
                width: thickness);

            // Draw Arrow
            var arrow = new Vector3[]
            {
                endPoint, new(endPoint.x - kArrowLength, endPoint.y - kArrowWidth * 0.5f, endPoint.z),
                new(endPoint.x - kArrowLength, endPoint.y + kArrowWidth * 0.5f, endPoint.z),
            };
            var previousColor = Handles.color;
            Handles.color = lineColor;
            Handles.DrawAAConvexPolygon(arrow);
            Handles.color = previousColor;

            // Draw number
            if (Data.ReferenceCount > 1 && _connectionSettings.DrawReferenceCount)
            {
                var previousGUIColor = GUI.color;
                GUI.color = new Color(lineColor.r, lineColor.g, lineColor.b, 1f);
                var labelPosition = endPoint - new Vector3(kArrowLength * 2, 0, 0);
                Handles.Label(labelPosition, $"{Data.ReferenceCount}");
                GUI.color = previousGUIColor;
            }

            Handles.EndGUI();
        }
    }
}
