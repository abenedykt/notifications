using System.Collections.Generic;

namespace Notifications.DataAccessLayer
{

    public class SqlEmployee
    {
        public int EmployeeId { get; set; }
        public string Name { get; set; }
        public bool IfLogin { get; set; }

        public List<SqlNotification> SendNotifications { get; set; }

        public List<SqlMessage> SendMessages { get; set; }

        public List<SqlMessage> ReceiveMessages { get; set; }

        public virtual List<SqlNotification> ReceiveNotification { get; set; }
    }
}
