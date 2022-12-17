// ****************
// Giant Particle Inc.
// All rights reserved.
// ****************

using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GiantParticle.InspectorGraph.ContentView
{
    public class IMGUIPreviewWindowContent : BaseWindowContent
    {
        private UnityEditor.Editor _editor;
        private IMGUIContainer _view;

        public IMGUIPreviewWindowContent(IWindowData windowData)
        {
            _editor = UnityEditor.Editor.CreateEditor(windowData.Target);
            ApplyHackForMaterialEditor();

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
            var rect = EditorGUILayout.BeginHorizontal();
            _editor.OnPreviewSettings();
            EditorGUILayout.EndHorizontal();
            _editor.OnPreviewGUI(new Rect(0, rect.height,
                localBound.width, localBound.height - rect.height), GUIStyle.none);
        }

        private void ApplyHackForMaterialEditor()
        {
            if (!(_editor is MaterialEditor materialEditor)) return;

            // This hack is to enable changing the shape of the preview mesh, otherwise
            // the mesh will not change when toggling the shape button.
            var property = typeof(MaterialEditor).GetProperty("firstInspectedEditor",
                BindingFlags.NonPublic | BindingFlags.Default | BindingFlags.Instance);
            if (property == null) return;
            property.SetValue(materialEditor, true);
        }
    }
}
