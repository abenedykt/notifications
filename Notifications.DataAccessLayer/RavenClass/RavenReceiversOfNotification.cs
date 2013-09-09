using System;

namespace Notifications.DataAccessLayer.RavenClass
{
    public class RavenReceiversOfNotification
    {
        public string Id { get; set; }
        public string NotificationId { get; set; }
        public string ReceiverId { get; set; }

        public DateTime Date { get; set; }
        public DateTime WhenRead { get; set; }
    }
}