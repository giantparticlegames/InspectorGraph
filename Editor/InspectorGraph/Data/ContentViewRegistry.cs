// ********************************
// (C) 2022 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using System.Collections.Generic;
using GiantParticle.InspectorGraph.Editor.Views;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace GiantParticle.InspectorGraph.Editor.Data
{
    internal interface IContentViewRegistry
    {
        int WindowCount { get; }
        InspectorWindow WindowByTarget(Object target);
        void ExecuteOnEachWindow(Action<InspectorWindow> action);
        IEnumerable<ConnectionLine> AllConnectionsRelatedToWindow(InspectorWindow window);
        IEnumerable<ConnectionLine> ConnectionsFromWindow(InspectorWindow source);
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
            _windowsByObject.Add(window.Node.Target, window);
        }

        public void DeregisterWindow(InspectorWindow window)
        {
            _windowsByObject.Remove(window.Node.Target);
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
                if (collection.Contains(window.Node.Target)) continue;
                yield return window;
            }
        }

        #endregion

        #region Connections

        public bool ContainsConnection(VisualElement source, VisualElement dest)
        {
            foreach (ConnectionLine connectionLine in _allLines)
            {
                if (connectionLine.Source != source) continue;
                if (connectionLine.Destination != dest) continue;
                return true;
            }

            return false;
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

        public IEnumerable<ConnectionLine> ConnectionsFromWindow(InspectorWindow source)
        {
            foreach (ConnectionLine line in _allLines)
            {
                if (line.Source != source) continue;
                yield return line;
            }
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
