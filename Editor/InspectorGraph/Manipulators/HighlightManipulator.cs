// ********************************
// (C) 2022 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using System.Collections.Generic;
using GiantParticle.InspectorGraph.Editor.Data;
using GiantParticle.InspectorGraph.Editor.Views;
using UnityEngine.UIElements;

namespace GiantParticle.InspectorGraph.Editor.Manipulators
{
    internal class HighlightManipulator : BaseHoverManipulator
    {
        [Flags]
        public enum Mode
        {
            References = 0b0001,
            ReferencedBy = 0b0010,
            All = References | ReferencedBy
        }

        private InspectorWindow _owner;
        private Mode _mode;
        private bool _isHovering;

        private bool _forceActive;

        public bool ForceActive
        {
            get => _forceActive;
            set
            {
                if (_forceActive == value) return;
                _forceActive = value;
                ForceHighlight();
            }
        }

        public HighlightManipulator(VisualElement handle, InspectorWindow owner, Mode mode) :
            base(handle)
        {
            this.target = handle;
            _mode = mode;
            _owner = owner;
            _isHovering = false;
        }

        protected override void OnHover(PointerEnterEvent evt)
        {
            this.target.AddToClassList("active");
            _isHovering = true;
            UpdateView();
        }

        protected override void OnExit(PointerLeaveEvent evt)
        {
            this.target.RemoveFromClassList("active");
            _isHovering = false;
            UpdateView();
        }

        private void UpdateView()
        {
            ConnectionLine.ViewMode viewMode = _isHovering
                ? ConnectionLine.ViewMode.Highlighted
                : ConnectionLine.ViewMode.Normal;

            IContentViewRegistry registry = GlobalApplicationContext.Instance.Get<IContentViewRegistry>();
            if (_mode.HasFlag(Mode.References))
                ProcessConnection(registry.ConnectionsFromWindow(_owner), viewMode);

            if (_mode.HasFlag(Mode.ReferencedBy))
                ProcessConnection(registry.ConnectionsToWindow(_owner), viewMode);
        }

        private void ForceHighlight()
        {
            IContentViewRegistry registry = GlobalApplicationContext.Instance.Get<IContentViewRegistry>();
            if (_mode.HasFlag(Mode.References))
                ForceHighlight(registry.ConnectionsFromWindow(_owner));

            if (_mode.HasFlag(Mode.ReferencedBy))
                ForceHighlight(registry.ConnectionsToWindow(_owner));
        }

        private void ForceHighlight(IEnumerable<ConnectionLine> references)
        {
            foreach (ConnectionLine line in references)
                line.ForceHighlightCount += _forceActive ? 1 : -1;
        }

        private void ProcessConnection(IEnumerable<ConnectionLine> references, ConnectionLine.ViewMode viewMode)
        {
            foreach (ConnectionLine line in references)
            {
                line.Mode = viewMode;
                switch (viewMode)
                {
                    case ConnectionLine.ViewMode.Normal:
                        line.SendToBack();
                        break;
                    case ConnectionLine.ViewMode.Highlighted:
                        line.BringToFront();
                        break;
                }
            }
        }
    }
}
