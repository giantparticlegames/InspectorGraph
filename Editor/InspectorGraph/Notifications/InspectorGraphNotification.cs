// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using GiantParticle.InspectorGraph.UIToolkit;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GiantParticle.InspectorGraph.Notifications
{
    internal partial class InspectorGraphNotification : VisualElement
    {
        public InspectorGraphNotification(NotificationType notificationType, string message)
        {
            var asset = UIToolkitHelper.LocateViewForType(this);
            if (asset == null) return;
            asset.CloneTree(this);
            UIToolkitHelper.ResolveVisualElements(this, this);

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
