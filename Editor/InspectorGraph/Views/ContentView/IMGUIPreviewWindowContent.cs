// ********************************
// (C) 2022 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using System.Reflection;
using GiantParticle.InspectorGraph.Data;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GiantParticle.InspectorGraph.ContentView
{
    internal class IMGUIPreviewWindowContent : BaseWindowContent
    {
        private const BindingFlags kStandardFlags = BindingFlags.NonPublic
                                                    | BindingFlags.Default
                                                    | BindingFlags.Instance;

        private UnityEditor.Editor _editor;
        private IMGUIContainer _view;

        public IMGUIPreviewWindowContent(IWindowData windowData)
        {
            _editor = UnityEditor.Editor.CreateEditor(windowData.Object);
            ApplyHacksForEditor();

            windowData.UpdateSerializedObject(_editor.serializedObject);
            _view = new IMGUIContainer(DrawEditorPreview);
            _view.style.minWidth = 128;
            _view.style.minHeight = 128;
            this.Add(_view);
            this.AddToClassList($"{ContentViewMode.Preview}");
        }

        public override void Dispose()
        {
            UnityEditor.Editor.DestroyImmediate(_editor);
            _editor = null;

            _view.RemoveFromHierarchy();
            _view.Dispose();
            _view = null;
        }

        private void DrawEditorPreview()
        {
            // TODO: Adjust space
            var settingsRect = EditorGUILayout.BeginHorizontal();
            _editor.OnPreviewSettings();
            EditorGUILayout.EndHorizontal();

            var previewRect = new Rect(0, settingsRect.height,
                localBound.width, localBound.height - settingsRect.height);
            _editor.OnInteractivePreviewGUI(previewRect, GUIStyle.none);

            if (ShouldRefreshConstantly())
                MarkDirtyRepaint();
        }

        private void ApplyHacksForEditor()
        {
            // [MaterialEditor]
            // This hack is to enable changing the shape of the preview mesh, otherwise
            // the mesh will not change when toggling the shape button.
            if (_editor is MaterialEditor materialEditor)
            {
                var property = typeof(MaterialEditor).GetProperty(
                    name: "firstInspectedEditor",
                    bindingAttr: kStandardFlags);
                if (property == null) return;
                property.SetValue(materialEditor, true);
                return;
            }

            // [AnimationClipEditor]
            // This hack is to enable preview without crashes
            if (string.Equals(_editor.GetType().Name, "AnimationClipEditor"))
            {
                Type editorType = _editor.GetType();
                MethodInfo initMethod = editorType.GetMethod(
                    name: "Init",
                    bindingAttr: kStandardFlags);
                if (initMethod != null) initMethod.Invoke(_editor, null);
            }
        }

        private bool ShouldRefreshConstantly()
        {
            if (string.Equals(_editor.GetType().Name, "AnimationClipEditor")) return true;
            return false;
        }
    }
}
