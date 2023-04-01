// ********************************
// (C) 2022 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GiantParticle.InspectorGraph.Editor
{
    internal class InspectorElementObserver
    {
        private InspectorElement _inspectorElement;
        private List<ObjectField> _registeredFields = new();
        private List<ListView> _registeredLists = new();
        private bool _isObservingGeometry;

        public event Action InspectorElementChanged;

        public void ObserveInspectorElement(InspectorElement element)
        {
            if (_inspectorElement != null) StopObservingInspector();
            _inspectorElement = element;
            if (_inspectorElement == null) return;

            ObserveGeometry();
            ObserveObjectFields();
            ObserverListFields();
        }

        public void StopObservingInspector()
        {
            if (_inspectorElement == null) return;

            StopObservingGeometry();
            StopObservingObjectFields();
            StopObservingListFields();
            _inspectorElement = null;
        }

        #region Geometry Observations

        private void ObserveGeometry()
        {
            if (_inspectorElement == null) return;
            if (_isObservingGeometry) return;

            _inspectorElement.RegisterCallback<GeometryChangedEvent>(OnInspectorGeometryChanged);
            _isObservingGeometry = true;
        }

        private void StopObservingGeometry()
        {
            if (_inspectorElement == null) return;
            if (!_isObservingGeometry) return;

            _inspectorElement.UnregisterCallback<GeometryChangedEvent>(OnInspectorGeometryChanged);
            _isObservingGeometry = false;
        }

        private void OnInspectorGeometryChanged(GeometryChangedEvent evt)
        {
            ObserveObjectFields();
            ObserverListFields();
        }

        #endregion

        #region ObjectField Observations

        private void ObserveObjectFields()
        {
            StopObservingObjectFields();

            var query = _inspectorElement.Query<ObjectField>().Build();
            foreach (ObjectField field in query)
            {
                field.RegisterCallback<ChangeEvent<UnityEngine.Object>>(OnObjectFieldChanged);
                _registeredFields.Add(field);
            }
        }

        private void StopObservingObjectFields()
        {
            for (int i = 0; i < _registeredFields.Count; ++i)
            {
                var field = _registeredFields[i];
                field.UnregisterCallback<ChangeEvent<UnityEngine.Object>>(OnObjectFieldChanged);
            }

            _registeredFields.Clear();
        }

        private void OnObjectFieldChanged(ChangeEvent<UnityEngine.Object> evt)
        {
            InspectorElementChanged?.Invoke();
        }

        #endregion


        #region ListView Observations

        private void ObserverListFields()
        {
            StopObservingListFields();
            var query = _inspectorElement.Query<ListView>().Build();
            foreach (ListView listView in query)
            {
                listView.itemsAdded += OnListViewChanged;
                listView.itemsRemoved += OnListViewChanged;
                _registeredLists.Add(listView);
            }
        }

        private void StopObservingListFields()
        {
            for (int i = 0; i < _registeredLists.Count; ++i)
            {
                var listView = _registeredLists[i];
                listView.itemsAdded -= OnListViewChanged;
                listView.itemsRemoved -= OnListViewChanged;
            }

            _registeredLists.Clear();
        }

        private void OnListViewChanged(IEnumerable<int> enumerable)
        {
            InspectorElementChanged?.Invoke();
        }

        #endregion
    }
}
