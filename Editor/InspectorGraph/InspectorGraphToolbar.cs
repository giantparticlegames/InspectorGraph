// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using GiantParticle.InspectorGraph.Data.Graph.Filters;
using GiantParticle.InspectorGraph.Persistence;
using GiantParticle.InspectorGraph.Plugins;
using GiantParticle.InspectorGraph.UIToolkit;
using GiantParticle.InspectorGraph.Views;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace GiantParticle.InspectorGraph
{
    internal class InspectorGraphToolbarConfig
    {
        public Action UpdateCallback;
        public Action ResetCallback;
        public Action<Object> CreateCallback;
    }

    internal partial class InspectorGraphToolbar : VisualElement, IDisposable
    {
        private const string kDocsURL = "https://github.com/giantparticlegames/InspectorGraph/blob/main/README.md";
        private const string kReportBugURL = "https://github.com/giantparticlegames/InspectorGraph/issues/new";
        private const string kWebsite = "https://www.giantparticlegames.com/home/inspector-graph";
        private InspectorGraphToolbarConfig _config;
        private IInspectorGraphToolbar[] _extensions;

        public InspectorGraphToolbar(InspectorGraphToolbarConfig config)
        {
            _config = config;
            _extensions = ReflectionHelper.InstantiateAllImplementations<IInspectorGraphToolbar>();
            ExecuteOnAllExtensions(toolbar =>
            {
                if (toolbar is IInspectorGraphToolbarConfigurable configurableToolbar)
                    configurableToolbar.Configure(_config);
            });
            LoadLayout();
            ConfigureUI();
        }

        private void ExecuteOnAllExtensions(Action<IInspectorGraphToolbar> action)
        {
            if (_extensions == null) return;
            for (int i = 0; i < _extensions.Length; ++i)
                action.Invoke(_extensions[i]);
        }

        public void Dispose()
        {
            ExecuteOnAllExtensions(toolbar =>
            {
                if (toolbar is IDisposable disposableToolbar)
                    disposableToolbar.Dispose();
            });
        }

        private void LoadLayout()
        {
            var asset = UIToolkitHelper.LocateViewForType(this);
            if (asset == null) return;
            asset.CloneTree(this);
            UIToolkitHelper.ResolveVisualElements(this, this);
        }

        private void ConfigureUI()
        {
            ConfigureViewMenu();
            ConfigureEditMenu();
            ConfigureHelpMenu();

            ConfigureActiveObject();
            ExecuteOnAllExtensions(toolbar => toolbar.ConfigureView(this));
        }

        private void ConfigureViewMenu()
        {
            // Refresh View
            _viewMenu.menu.AppendAction(
                actionName: "Refresh",
                action: (menuAction) => _config.UpdateCallback?.Invoke());
            _viewMenu.menu.AppendAction(
                actionName: "Reset",
                action: (menuAction) => _config.ResetCallback?.Invoke());
            _viewMenu.menu.AppendSeparator();

            // Filters
            var typeFilterHandler = GlobalApplicationContext.Instance.Get<ITypeFilterHandler>();
            _viewMenu.menu.AppendAction(
                actionName: "Filters/Enable Filters",
                action: action =>
                {
                    typeFilterHandler.EnableFilters = !typeFilterHandler.EnableFilters;
                    // Refresh view if needed
                    _config.UpdateCallback?.Invoke();
                },
                actionStatusCallback: action =>
                {
                    return typeFilterHandler.EnableFilters
                        ? DropdownMenuAction.Status.Checked
                        : DropdownMenuAction.Status.Normal;
                });
            _viewMenu.menu.AppendSeparator("Filters/");
            foreach (ITypeFilter filter in typeFilterHandler.Filters)
            {
                // Show
                _viewMenu.menu.AppendAction(
                    actionName: $"Filters/{filter.TargetType.FullName}/Show",
                    action: (menuAction) =>
                    {
                        filter.ShouldShowType = !filter.ShouldShowType;
                        // Refresh view if needed
                        _config.UpdateCallback?.Invoke();
                    },
                    actionStatusCallback: action =>
                    {
                        if (filter.ShouldShowType)
                            return DropdownMenuAction.Status.Checked;
                        return DropdownMenuAction.Status.Normal;
                    });
                // Expand
                _viewMenu.menu.AppendAction(
                    actionName: $"Filters/{filter.TargetType.FullName}/Expand",
                    action: (menuAction) =>
                    {
                        filter.ShouldExpandType = !filter.ShouldExpandType;
                        // Refresh view if needed
                        _config.UpdateCallback?.Invoke();
                    },
                    actionStatusCallback: action =>
                    {
                        if (filter.ShouldExpandType)
                            return DropdownMenuAction.Status.Checked;
                        return DropdownMenuAction.Status.Normal;
                    });
            }
        }

        private void ConfigureEditMenu()
        {
            // Open project settings
            _editMenu.menu.AppendAction(
                actionName: "Project Settings",
                action: (menuAction) =>
                {
                    SettingsService.OpenProjectSettings($"{InspectorGraphSettingsProvider.kMenuPath}");
                });
        }

        private void ConfigureHelpMenu()
        {
            _helpMenu.menu.AppendAction(
                actionName: "Documentation",
                action: (menuAction) => { Application.OpenURL(kDocsURL); });
            _helpMenu.menu.AppendAction(
                actionName: "Report a bug",
                action: (menuAction) => { Application.OpenURL(kReportBugURL); });
            _helpMenu.menu.AppendAction(
                actionName: "Website",
                action: (menuAction) => { Application.OpenURL(kWebsite); });
        }

        private void ConfigureActiveObject()
        {
            _refField.objectType = typeof(Object);
            _refField.RegisterCallback<ChangeEvent<Object>>(evt =>
            {
                var assignedObject = evt.newValue;
                var preferences = GlobalApplicationContext.Instance.Get<IInspectorGraphUserPreferences>();
                if (assignedObject == null) preferences.LastInspectedObjectGUID = null;
                else
                {
                    string path = AssetDatabase.GetAssetPath(assignedObject);
                    string guid = AssetDatabase.AssetPathToGUID(path);
                    preferences.LastInspectedObjectGUID = guid;
                    preferences.Save();
                }

                _config.CreateCallback?.Invoke(assignedObject);
            });
        }

        public void LoadPreferences()
        {
            // Load last saved object
            var preferences = GlobalApplicationContext.Instance.Get<IInspectorGraphUserPreferences>();
            if (string.IsNullOrEmpty(preferences.LastInspectedObjectGUID)) return;

            var path = AssetDatabase.GUIDToAssetPath(preferences.LastInspectedObjectGUID);
            if (string.IsNullOrEmpty(path)) return;

            var lastObject = AssetDatabase.LoadAssetAtPath<Object>(path);
            if (lastObject == null) return;

            _refField.value = lastObject;

            ExecuteOnAllExtensions(toolbar =>
            {
                if (toolbar is IConfigurablePreferences configurableToolbar)
                    configurableToolbar.ConfigurePreferences();
            });
        }
    }
}
