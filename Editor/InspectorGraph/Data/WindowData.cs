// ********************************
// (C) 2022 - Giant Particle Games
// All rights reserved.
// ********************************

using UnityEditor;
using UnityEngine;

namespace GiantParticle.InspectorGraph.Editor.Data
{
    internal interface IWindowData
    {
        Object Target { get; }
        SerializedObject SerializedTarget { get; }
        bool HasBeenManuallyMoved { get; set; }

        void CreateNewSerializedTarget();
        void UpdateSerializedObject(SerializedObject newSerializedObject);
    }

    internal class WindowData : IWindowData
    {
        public Object Target { get; }

        public SerializedObject SerializedTarget
        {
            get
            {
                if (_serializedObject == null)
                    _serializedObject = new SerializedObject(Target);
                return _serializedObject;
            }
            private set => _serializedObject = value;
        }

        public bool HasBeenManuallyMoved { get; set; }
        private SerializedObject _serializedObject;

        public WindowData(Object reference)
        {
            Target = reference;
        }

        public void CreateNewSerializedTarget()
        {
            SerializedTarget = new SerializedObject(Target);
        }

        public void UpdateSerializedObject(SerializedObject newSerializedObject)
        {
            SerializedTarget = newSerializedObject;
        }
    }
}
