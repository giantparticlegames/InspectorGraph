// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using System.Collections.Generic;
using GiantParticle.InspectorGraph.UIToolkit;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GiantParticle.InspectorGraph.PropertyDrawers
{
    internal class FilterTypeOptionsPopup : PopupWindowContent
    {
        private const int kItemHeight = 16;
        private const int kWindowWidth = 300;
        private const int kWindowHeight = 300;

        [VisualElementField]
        private ToolbarSearchField _searchField;

        private readonly List<Type> _allOptions;
        private List<Type> _filteredOptions;
        private readonly Action<Type> _selectionCallback;
        private ListView _listView;

        public FilterTypeOptionsPopup(List<Type> options, Action<Type> onSelection)
        {
            _allOptions = options;
            _filteredOptions = _allOptions;
            _selectionCallback = onSelection;
        }

        public override void OnGUI(Rect rect)
        {
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(kWindowWidth, kWindowHeight);
        }

        public override void OnOpen()
        {
            var asset = UIToolkitHelper.LocateViewForType(this);
            if (asset == null) return;
            asset.CloneTree(editorWindow.rootVisualElement);
            UIToolkitHelper.ResolveVisualElements(this, editorWindow.rootVisualElement);

            _searchField.RegisterValueChangedCallback(OnSearchChanged);

            // Add list
            _listView = new ListView(_filteredOptions, kItemHeight, MakeItem, BindItem);
            _listView.selectionType = SelectionType.Single;
            _listView.onItemsChosen += objects =>
            {
                foreach (object o in objects)
                    _selectionCallback?.Invoke((Type)o);
                editorWindow.Close();
            };
            _listView.horizontalScrollingEnabled = true;
            editorWindow.rootVisualElement.Add(_listView);

            _searchField.Focus();
            base.OnOpen();
        }

        private VisualElement MakeItem()
        {
            return new Label();
        }

        private void BindItem(VisualElement element, int index)
        {
            Label label = (Label)element;
            label.text = _filteredOptions[index].FullName;
        }

        private void OnSearchChanged(ChangeEvent<string> evt)
        {
            if (string.IsNullOrEmpty(evt.newValue) || evt.newValue.Length < 2)
            {
                if (_filteredOptions != _allOptions)
                    _filteredOptions.Clear();
                _filteredOptions = _allOptions;
                _listView.itemsSource = _filteredOptions;
                _listView.RefreshItems();
                return;
            }

            // Filter
            string searchTerm = evt.newValue;
            if (_filteredOptions == _allOptions) _filteredOptions = new List<Type>();
            _filteredOptions.Clear();
            for (int i = 0; i < _allOptions.Count; ++i)
            {
                Type type = _allOptions[i];
                if (type == null) continue;
                if (string.IsNullOrEmpty(type.FullName)) continue;
                if (!type.FullName.Contains(searchTerm)) continue;

                _filteredOptions.Add(type);
            }

            _listView.itemsSource = _filteredOptions;
            _listView.RefreshItems();
        }
    }
}
