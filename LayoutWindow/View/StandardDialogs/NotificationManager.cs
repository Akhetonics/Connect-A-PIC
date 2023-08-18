using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectAPIC.LayoutWindow.View
{
    public partial class NotificationManager : Node
    {
        private static NotificationManager _instance;
        public static NotificationManager Instance { get { return _instance; } }
        private List<NotificationBox> currentlyShownNotifications = new();
        [Export] public PackedScene NotificationBoxScene;

        public override void _Ready()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            if (NotificationBoxScene != null) GD.PrintErr($"{nameof(NotificationBoxScene)} is not attached to node");
        }

        public void Notify(string message, bool isError = false)
        {
            RemoveOldNotificationsFromList();
            
            var notificationBoxNode = NotificationBoxScene.Instantiate();
            var notificationBox = (NotificationBox)notificationBoxNode;
            notificationBox.SetAnchorsPreset(Control.LayoutPreset.TopRight);
            notificationBox.SetAnchorsPreset(Control.LayoutPreset.BottomRight);
            var yOffset = currentlyShownNotifications.Count * notificationBox.Size.Y;
            notificationBox.Position = new Vector2( notificationBox.Position.X, notificationBox.Position.Y - yOffset);
            currentlyShownNotifications.Add(notificationBox);
            GetTree().Root.AddChild(notificationBox);
            notificationBox.ShowMessage(message, isError);
        }

        private void RemoveOldNotificationsFromList()
        {
            List<NotificationBox> notificationsToRemove = currentlyShownNotifications
                         .Where(notification => isDisposed(notification))
                         .ToList();

            foreach (var notificationToRemove in notificationsToRemove)
            {
                currentlyShownNotifications.Remove(notificationToRemove);
            }
        }
        private bool isDisposed(Node obj)
        {
            if (obj == null) return true;
            try
            {
                if (obj.IsQueuedForDeletion())
                {
                    return true;
                }
            }
            catch
            {
                return true;
            }
            return false;
        }
    }
}
