using System;

namespace Notifications.DataAccessLayer.SqlClass
{
    public class SqlReceiversOfNotification
    {
        public SqlReceiversOfNotification()
        {
            ReceivingNotification = new SqlNotification();
        }

        public string ReceiversOfNotificationId { get; set; }
        public string NotificationId { get; set; }
        public string ReceiverId { get; set; }

        public SqlNotification ReceivingNotification { get; set; }
        public SqlEmployee Receiver { get; set; }

        public DateTime WhenRead { get; set; }
    }
}