// ********************************
// (C) 2022 - Giant Particle Games
// All rights reserved.
// ********************************

using UnityEditor;
using UnityEngine;

namespace GiantParticle.InspectorGraph.Data
{
    internal interface IWindowData
    {
        Object Object { get; }
        SerializedObject SerializedObject { get; }
        bool HasBeenManuallyMoved { get; set; }

        void CreateNewSerializedTarget();
        void UpdateSerializedObject(SerializedObject newSerializedObject);
    }

    internal class WindowData : IWindowData
    {
        public Object Object { get; }

        public SerializedObject SerializedObject
        {
            get
            {
                if (_serializedObject == null)
                    _serializedObject = new SerializedObject(Object);
                return _serializedObject;
            }
            private set => _serializedObject = value;
        }

        public bool HasBeenManuallyMoved { get; set; }
        private SerializedObject _serializedObject;

        public WindowData(Object reference)
        {
            Object = reference;
        }

        public void CreateNewSerializedTarget()
        {
            SerializedObject = new SerializedObject(Object);
        }

        public void UpdateSerializedObject(SerializedObject newSerializedObject)
        {
            SerializedObject = newSerializedObject;
        }
    }
}
