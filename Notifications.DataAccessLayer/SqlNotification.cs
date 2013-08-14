using System;
using System.Collections.Generic;

namespace Notifications.DataAccessLayer
{
    public class SqlNotification
    {
        public int NotificationId { get; set; }
        public DateTime Date { get; set; }
        public string Content { get; set; }

        public SqlEmployee Sender { get; set; }
        public int SenderId { get; set; }

        public virtual List<SqlEmployee> Receivers { get; set; }
    }
}