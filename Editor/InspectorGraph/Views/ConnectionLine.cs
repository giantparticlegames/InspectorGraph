// ********************************
// (C) 2022 - Giant Particle Games
// All rights reserved.
// ********************************

using GiantParticle.InspectorGraph.Editor.Data.Nodes;
using GiantParticle.InspectorGraph.Editor.Settings;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace GiantParticle.InspectorGraph.Editor.Views
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
        public int ForceHighlightCount { get; set; }

        private ViewMode _mode;
        private IReferenceColorSettings _colorSettings;

        public ConnectionLine(VisualElement source, VisualElement dest, ReferenceType refType) : base()
        {
            Source = source;
            Destination = dest;
            var settings = GlobalApplicationContext.Instance.Get<IInspectorGraphSettings>();
            _colorSettings = settings.GetReferenceColor(refType);

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
                                 new Vector3(Source.contentRect.width, Source.contentRect.height * 0.5f, 0);
            Vector3 endPoint = Destination.transform.position +
                               new Vector3(0, Destination.contentRect.height * 0.5f, 0);

            Handles.DrawBezier(startPosition: startPoint,
                endPosition: endPoint - Vector3.right * 10,
                startTangent: startPoint + Vector3.right * 25,
                endTangent: endPoint + Vector3.left * 25,
                color: lineColor,
                texture: Texture2D.whiteTexture,
                width: thickness);
            var arrow = new Vector3[]
            {
                endPoint, new(endPoint.x - kArrowLength, endPoint.y - kArrowWidth * 0.5f, endPoint.z),
                new(endPoint.x - kArrowLength, endPoint.y + kArrowWidth * 0.5f, endPoint.z),
            };
            Handles.color = lineColor;
            Handles.DrawAAConvexPolygon(arrow);
            Handles.EndGUI();
        }
    }
}
