// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using GiantParticle.InspectorGraph.Editor.Preferences;
using GiantParticle.InspectorGraph.Editor.UIDocuments;
using GiantParticle.InspectorGraph.Editor.Data.Nodes.Filters;
using GiantParticle.InspectorGraph.Editor.Settings;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace GiantParticle.InspectorGraph.Editor
{
    internal class InspectorGraphToolbarConfig
    {
        public Action UpdateCallback;
        public Action ResetCallback;
        public Action<Object> CreateCallback;
    }

    internal interface IInspectorGraphToolbarExtension : IDisposable
    {
        void Configure(VisualElement root);
        void LoadPreferences();
    }

    internal class InspectorGraphToolbar : VisualElement, IDisposable
    {
        private const string kDocsURL = "https://github.com/giantparticlegames/InspectorGraph/blob/main/README.md";
        private const string kReportBugURL = "https://github.com/giantparticlegames/InspectorGraph/issues/new";
        private const string kWebsite = "https://www.giantparticlegames.com/home/inspector-graph";
        private InspectorGraphToolbarConfig _config;
        private ObjectField _objectField;
        private IInspectorGraphToolbarExtension[] _extensions;

        public InspectorGraphToolbar(InspectorGraphToolbarConfig config)
        {
            _config = config;
            _extensions = ReflectionHelper.InstantiateAllImplementations<IInspectorGraphToolbarExtension>();
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
        }

        private void ConfigureUI()
        {
            ConfigureViewMenu();
            ConfigureEditMenu();
            ConfigureHelpMenu();

            ConfigureActiveObject();
            if (_extensions != null)
                for (int i = 0; i < _extensions.Length; ++i)
                    _extensions[i].Configure(this);
        }

        private void ConfigureViewMenu()
        {
            var viewMenu = this.Q<ToolbarMenu>("_viewMenu");
            // Refresh View
            viewMenu.menu.AppendAction(
                actionName: "Refresh",
                action: (menuAction) => _config.UpdateCallback?.Invoke());
            viewMenu.menu.AppendAction(
                actionName: "Reset",
                action: (menuAction) => _config.ResetCallback?.Invoke());
            viewMenu.menu.AppendSeparator();

            // Filters
            var typeFilterHandler = GlobalApplicationContext.Instance.Get<ITypeFilterHandler>();
            foreach (ITypeFilter filter in typeFilterHandler.Filters)
            {
                // Show
                viewMenu.menu.AppendAction(
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
                viewMenu.menu.AppendAction(
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
            var editMenu = this.Q<ToolbarMenu>("_editMenu");
            // Open project settings
            editMenu.menu.AppendAction(
                actionName: "Project Settings",
                action: (menuAction) =>
                {
                    SettingsService.OpenProjectSettings($"{InspectorGraphSettingsRegister.kMenuPath}");
                });
        }

        private void ConfigureHelpMenu()
        {
            var helpMenu = this.Q<ToolbarMenu>("_helpMenu");
            helpMenu.menu.AppendAction(
                actionName: "Documentation",
                action: (menuAction) => { Application.OpenURL(kDocsURL); });
            helpMenu.menu.AppendAction(
                actionName: "Report a bug",
                action: (menuAction) => { Application.OpenURL(kReportBugURL); });
            helpMenu.menu.AppendAction(
                actionName: "Website",
                action: (menuAction) => { Application.OpenURL(kWebsite); });
        }

        private void ConfigureActiveObject()
        {
            _objectField = this.Q<ObjectField>("_refField");
            _objectField.objectType = typeof(Object);
            _objectField.RegisterCallback<ChangeEvent<Object>>(evt =>
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

            _objectField.value = lastObject;

            if (_extensions != null)
                for (int i = 0; i < _extensions.Length; ++i)
                    _extensions[i].LoadPreferences();
        }
    }
}
