// ********************************
// (C) 2022 - Giant Particle Games
// All rights reserved.
// ********************************

using System.Collections.Generic;
using GiantParticle.InspectorGraph.Editor.Data;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GiantParticle.InspectorGraph.Editor.ContentView
{
    internal class IMGUIWindowContent : BaseWindowContent
    {
        private UnityEditor.Editor _editor;
        private List<UnityEditor.Editor> _editors;
        private IMGUIContainer _view;

        public IMGUIWindowContent(IWindowData windowData)
        {
            if (windowData.Target is GameObject gameObject)
            {
                var components = gameObject.GetComponents<Component>();
                _editors = new List<UnityEditor.Editor>(components.Length);
                for (int i = 0; i < components.Length; ++i)
                {
                    var editor = UnityEditor.Editor.CreateEditor(components[i]);
                    _editors.Add(editor);
                }
            }
            else
            {
                _editor = UnityEditor.Editor.CreateEditor(windowData.Target);
                windowData.UpdateSerializedObject(_editor.serializedObject);
            }

            _view = new IMGUIContainer(DrawEditor);
            this.Add(_view);
            this.AddToClassList($"{ContentViewMode.IMGUI}");
        }

        public override void Dispose()
        {
            if (_editor != null)
            {
                UnityEditor.Editor.DestroyImmediate(_editor);
                _editor = null;
            }

            if (_editors != null && _editors.Count > 0)
            {
                for (int i = 0; i < _editors.Count; ++i)
                    UnityEditor.Editor.DestroyImmediate(_editors[i]);
                _editors.Clear();
            }

            _view.RemoveFromHierarchy();
            _view.Dispose();
            _view = null;
        }

        private void DrawEditor()
        {
            EditorGUI.BeginChangeCheck();
            if (_editor != null) _editor.OnInspectorGUI();
            if (_editors != null && _editors.Count > 0)
            {
                for (int i = 0; i < _editors.Count; ++i)
                    _editors[i].OnInspectorGUI();
            }

            if (EditorGUI.EndChangeCheck())
                RaiseContentChanged();
        }
    }
}
