// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using UnityEngine.UIElements;

namespace GiantParticle.InspectorGraph.Notifications
{
    internal interface INotificationController
    {
        VisualElement Container { get; set; }
        void ShowNotification(NotificationType notificationType, string message);
        void ClearNotifications();
    }
}
