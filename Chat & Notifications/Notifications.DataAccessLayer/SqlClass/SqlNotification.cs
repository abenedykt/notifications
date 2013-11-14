using System;
using System.Collections.Generic;

namespace Notifications.DataAccessLayer.SqlClass
{
    public class SqlNotification
    {
        public SqlNotification()
        {
            Receivers = new List<SqlReceiversOfNotification>();
        }

        public string NotificationId { get; set; }
        public DateTime Date { get; set; }
        public string Content { get; set; }

        public SqlEmployee Sender { get; set; }
        public string SenderId { get; set; }

        public List<SqlReceiversOfNotification> Receivers { get; set; }
    }
}