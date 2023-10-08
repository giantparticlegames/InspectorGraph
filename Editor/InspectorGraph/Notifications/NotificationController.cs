// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using UnityEngine.UIElements;

namespace GiantParticle.InspectorGraph.Notifications
{
    internal class NotificationController : INotificationController
    {
        public VisualElement Container { get; set; }

        public void ShowNotification(NotificationType notificationType, string message)
        {
            var notification = new InspectorGraphNotification(notificationType, message);
            Container.Add(notification);
        }

        public void ClearNotifications()
        {
            Container?.Clear();
        }
    }
}
