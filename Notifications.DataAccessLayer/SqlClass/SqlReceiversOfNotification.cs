using System;

namespace Notifications.DataAccessLayer.SqlClass
{
    public class SqlReceiversOfNotification
    {
        public SqlReceiversOfNotification()
        {
            ReceivingNotification = new SqlNotification();
        }

        public int ReceiversOfNotificationId { get; set; }
        public int NotificationId { get; set; }
        public int ReceiverId { get; set; }

        public SqlNotification ReceivingNotification { get; set; }
        public SqlEmployee Receiver { get; set; }

        public DateTime WhenRead { get; set; }
    }
}