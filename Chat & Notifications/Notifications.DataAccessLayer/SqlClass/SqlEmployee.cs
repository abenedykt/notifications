using System.Collections.Generic;

namespace Notifications.DataAccessLayer.SqlClass
{
    public class SqlEmployee
    {
        public int EmployeeId { get; set; }
        public string Name { get; set; }

        public virtual List<SqlNotification> SendNotifications { get; set; }
        public virtual List<SqlMessage> SendMessages { get; set; }
        public virtual List<SqlMessage> ReceiveMessages { get; set; }
        public virtual List<SqlReceiversOfNotification> ReceiveNotifications { get; set; }
    }
}