// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using GiantParticle.InspectorGraph.CustomAttributes;
using GiantParticle.InspectorGraph.Preferences;
using GiantParticle.InspectorGraph.UIDocuments;
using GiantParticle.InspectorGraph.Data.Graph.Filters;
using GiantParticle.InspectorGraph.Editor.InspectorGraph.Data.Graph;
using GiantParticle.InspectorGraph.Plugins;
using GiantParticle.InspectorGraph.Settings;
using UnityEditor;
using UnityEditor.UIElements;
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
        private IInspectorGraphToolbarPlugin[] _extensions;

        public InspectorGraphToolbar(InspectorGraphToolbarConfig config)
        {
            _config = config;
            _extensions = ReflectionHelper.InstantiateAllImplementations<IInspectorGraphToolbarPlugin>();
            LoadLayout();
            ConfigureUI();
        }

        public void Dispose()
        {
            if (_extensions != null)
                for (int i = 0; i < _extensions.Length; ++i)
                    _extensions[i].Dispose();
        }

        private void LoadLayout()
        {
            var catalog = GlobalApplicationContext.Instance.Get<IUIDocumentCatalog<MainWindowUIDocumentType>>();
            IUIDocumentInfo<MainWindowUIDocumentType> info = catalog[MainWindowUIDocumentType.MainWindowToolbar];
            info.Asset.CloneTree(this);
            AssignVisualElements();
        }

        private void ConfigureUI()
        {
            ConfigureViewMenu();
            ConfigureEditMenu();
            ConfigureHelpMenu();

            ConfigureActiveMode();
            ConfigureActiveObject();
            if (_extensions != null)
                for (int i = 0; i < _extensions.Length; ++i)
                    _extensions[i].ConfigureView(this);
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
                    SettingsService.OpenProjectSettings($"{InspectorGraphSettingsRegister.kMenuPath}");
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

        private void ConfigureActiveMode()
        {
            var controller = GlobalApplicationContext.Instance.Get<IGraphController>();
            _inspectionModeDropdown.choices.Clear();
            for (int i = 0; i < controller.AvailableFactories.Length; ++i)
            {
                var factory = controller.AvailableFactories[i];
                var displayName = (EditorDisplayNameAttribute)Attribute.GetCustomAttribute(
                    element: factory.GetType(),
                    attributeType: typeof(EditorDisplayNameAttribute));
                _inspectionModeDropdown.choices.Add(displayName.DisplayName);
            }

            _inspectionModeDropdown.index = 0;

            _inspectionModeDropdown.visible = _inspectionModeDropdown.choices.Count > 1;
            _inspectionModeDropdown.RegisterValueChangedCallback(evt =>
            {
                int index = _inspectionModeDropdown.choices.IndexOf(evt.newValue);
                controller.SelectFactory(index);
                _config?.ResetCallback.Invoke();
            });
        }

        private void ConfigureActiveObject()
        {
            _refField.objectType = typeof(Object);
            _refField.RegisterCallback<ChangeEvent<Object>>(evt =>
            {
                var assignedObject = evt.newValue;
                IPreferenceHandler handler = GlobalApplicationContext.Instance.Get<IPreferenceHandler>();
                GeneralPreferences generalPrefs = handler.GetPreference<GeneralPreferences>();

                if (assignedObject == null) generalPrefs.LastInspectedObjectGUID = null;
                else
                {
                    string path = AssetDatabase.GetAssetPath(assignedObject);
                    string guid = AssetDatabase.AssetPathToGUID(path);
                    generalPrefs.LastInspectedObjectGUID = guid;
                    handler.Save<GeneralPreferences>();
                }

                _config.CreateCallback?.Invoke(assignedObject);
            });
        }

        public void LoadPreferences()
        {
            // Load last saved object
            IPreferenceHandler handler = GlobalApplicationContext.Instance.Get<IPreferenceHandler>();
            GeneralPreferences generalPrefs = handler.GetPreference<GeneralPreferences>();
            if (string.IsNullOrEmpty(generalPrefs.LastInspectedObjectGUID)) return;

            var path = AssetDatabase.GUIDToAssetPath(generalPrefs.LastInspectedObjectGUID);
            if (string.IsNullOrEmpty(path)) return;

            var lastObject = AssetDatabase.LoadAssetAtPath<Object>(path);
            if (lastObject == null) return;

            _refField.value = lastObject;

            if (_extensions != null)
                for (int i = 0; i < _extensions.Length; ++i)
                    _extensions[i].ConfigurePreferences(handler);
        }
    }
}
