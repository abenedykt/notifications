using System.Collections.Generic;

namespace Notifications.DataAccessLayer.RavenClass
{
    public class RavenEmployee
    {
        public int EmployeeId { get; set; }
        public string Name { get; set; }

        public List<RavenNotification> SendNotifications { get; set; }
        public List<RavenMessage> SendMessages { get; set; }
        public List<RavenMessage> ReceiveMessages { get; set; }
        public List<RavenReceiversOfNotification> ReceiveNotifications { get; set; }
    }
}