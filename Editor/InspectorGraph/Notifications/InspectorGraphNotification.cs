// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using GiantParticle.InspectorGraph.UIDocuments;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GiantParticle.InspectorGraph.Notifications
{
    internal partial class InspectorGraphNotification : VisualElement
    {
        public InspectorGraphNotification(NotificationType notificationType, string message)
        {
            var catalog = GlobalApplicationContext.Instance.Get<IUIDocumentCatalog<MainWindowUIDocumentType>>();
            IUIDocumentInfo<MainWindowUIDocumentType> info = catalog[MainWindowUIDocumentType.MainWindowNotification];
            info.Asset.CloneTree(this);
            AssignVisualElements();

            _text.text = message;
            _closeButton.RegisterCallback<ClickEvent>(CloseButton_OnClickEvent);
            UpdateIcon(notificationType);
        }

        private void UpdateIcon(NotificationType notificationType)
        {
            Texture2D image = null;
            switch (notificationType)
            {
                case NotificationType.Info:
                    image = EditorGUIUtility.FindTexture("console.infoicon");
                    break;
                case NotificationType.Warning:
                    image = EditorGUIUtility.FindTexture("console.warnicon");
                    break;
                case NotificationType.Error:
                    image = EditorGUIUtility.FindTexture("console.erroricon");
                    break;
            }

            _icon.style.backgroundImage = new StyleBackground(image);
        }

        private void CloseButton_OnClickEvent(ClickEvent evt)
        {
            this.parent.Remove(this);
        }
    }
}
