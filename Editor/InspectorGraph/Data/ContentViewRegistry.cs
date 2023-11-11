// ********************************
// (C) 2022 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using System.Collections.Generic;
using GiantParticle.InspectorGraph.Data.Nodes;
using GiantParticle.InspectorGraph.Views;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace GiantParticle.InspectorGraph.Data
{
    internal interface IContentViewRegistry
    {
        int WindowCount { get; }
        InspectorWindow WindowByTarget(Object target);
        void ExecuteOnEachWindow(Action<InspectorWindow> action);
        bool ContainsConnection(VisualElement source, VisualElement dest, ReferenceType refType);
        ConnectionLine GetConnection(VisualElement source, VisualElement dest, ReferenceType refType);
        void RegisterConnection(ConnectionLine line);
        IEnumerable<ConnectionLine> AllConnectionsRelatedToWindow(InspectorWindow window);
        int ConnectionsFromWindowCount(InspectorWindow source);
        IEnumerable<ConnectionLine> ConnectionsFromWindow(InspectorWindow source);
        int ConnectionsToWindowCount(InspectorWindow source);
        IEnumerable<ConnectionLine> ConnectionsToWindow(InspectorWindow destination);
    }

    internal class ContentViewRegistry : IContentViewRegistry
    {
        private Dictionary<Object, InspectorWindow> _windowsByObject = new();
        private HashSet<ConnectionLine> _allLines = new();

        public int WindowCount => _windowsByObject.Count;
        public IEnumerable<InspectorWindow> Windows => _windowsByObject.Values;

        public void Clear()
        {
            _allLines.Clear();
            _windowsByObject.Clear();
        }

        public void ClearConnections() => _allLines.Clear();

        #region Windows

        public InspectorWindow WindowByTarget(Object target)
        {
            if (!_windowsByObject.ContainsKey(target)) return null;
            return _windowsByObject[target];
        }

        public bool IsWindowRegisteredByTarget(Object target)
        {
            return _windowsByObject.ContainsKey(target);
        }

        public void RegisterWindow(InspectorWindow window)
        {
            _windowsByObject.Add(window.Node.Object, window);
        }

        public void DeregisterWindow(InspectorWindow window)
        {
            _windowsByObject.Remove(window.Node.Object);
        }

        public void DeregisterWindowByTarget(Object target)
        {
            _windowsByObject.Remove(target);
        }

        public void ExecuteOnEachWindow(Action<InspectorWindow> action)
        {
            foreach (InspectorWindow window in _windowsByObject.Values)
                action.Invoke(window);
        }

        public IEnumerable<InspectorWindow> AllWindowsExceptByKey(ICollection<Object> collection)
        {
            foreach (InspectorWindow window in _windowsByObject.Values)
            {
                if (collection.Contains(window.Node.Object)) continue;
                yield return window;
            }
        }

        #endregion

        #region Connections

        public bool ContainsConnection(VisualElement source, VisualElement dest, ReferenceType refType)
        {
            foreach (ConnectionLine connectionLine in _allLines)
            {
                if (connectionLine.Source != source) continue;
                if (connectionLine.Destination != dest) continue;
                if (connectionLine.ReferenceType != refType) continue;
                return true;
            }

            return false;
        }

        public ConnectionLine GetConnection(VisualElement source, VisualElement dest, ReferenceType refType)
        {
            foreach (ConnectionLine connectionLine in _allLines)
            {
                if (connectionLine.Source != source) continue;
                if (connectionLine.Destination != dest) continue;
                if (connectionLine.ReferenceType != refType) continue;
                return connectionLine;
            }

            return null;
        }

        public void RegisterConnection(ConnectionLine line)
        {
            _allLines.Add(line);
        }

        public void DeregisterConnection(ConnectionLine line)
        {
            _allLines.Remove(line);
        }

        public void DeregisterConnectionsRelatedToWindow(InspectorWindow window)
        {
            _allLines.RemoveWhere(line => line.Source == window || line.Destination == window);
        }

        public void ExecuteOnEachConnection(Action<ConnectionLine> action)
        {
            foreach (ConnectionLine line in _allLines)
                action.Invoke(line);
        }

        public void ExecuteOnEachConnectionByWindow(Action<ConnectionLine> action, InspectorWindow window)
        {
            foreach (ConnectionLine line in _allLines)
            {
                if (line.Source != window && line.Destination != window) continue;
                action.Invoke(line);
            }
        }

        public IEnumerable<ConnectionLine> AllConnectionsRelatedToWindow(InspectorWindow window)
        {
            foreach (ConnectionLine line in _allLines)
            {
                if (line.Source != window && line.Destination != window) continue;
                yield return line;
            }
        }

        public int ConnectionsFromWindowCount(InspectorWindow source)
        {
            int count = 0;
            foreach (ConnectionLine line in _allLines)
            {
                if (line.Source != source) continue;
                ++count;
            }

            return count;
        }

        public IEnumerable<ConnectionLine> ConnectionsFromWindow(InspectorWindow source)
        {
            foreach (ConnectionLine line in _allLines)
            {
                if (line.Source != source) continue;
                yield return line;
            }
        }

        public int ConnectionsToWindowCount(InspectorWindow destination)
        {
            int count = 0;
            foreach (ConnectionLine line in _allLines)
            {
                if (line.Destination != destination) continue;
                ++count;
            }

            return count;
        }

        public IEnumerable<ConnectionLine> ConnectionsToWindow(InspectorWindow destination)
        {
            foreach (ConnectionLine line in _allLines)
            {
                if (line.Destination != destination) continue;
                yield return line;
            }
        }

        #endregion
    }
}
