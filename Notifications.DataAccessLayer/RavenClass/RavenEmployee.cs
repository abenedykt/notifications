using System.Collections.Generic;

namespace Notifications.DataAccessLayer.RavenClass
{
    public class RavenEmployee
    {

        public RavenEmployee()
        {
            SendNotifications= new List<RavenNotification>();
            SendMessages = new List<RavenMessage>();
            ReceiveMessages = new List<RavenMessage>();
            ReceiveNotifications = new List<RavenNotification>();

        }

        public int EmployeeId { get; set; }
        public string Name { get; set; }

        public List<RavenNotification> SendNotifications { get; set; }
        public List<RavenMessage> SendMessages { get; set; }
        public List<RavenMessage> ReceiveMessages { get; set; }
        public List<RavenNotification> ReceiveNotifications { get; set; }
    }
}