using System;

namespace Notifications.DataAccessLayer.RavenClass
{
    public class RavenReceiversOfNotification
    {
        public int ReceiversOfNotificationId { get; set; }
        public RavenNotification Notification { get; set; }
        public RavenEmployee Receiver { get; set; }

        public DateTime WhenRead { get; set; }
    }
}