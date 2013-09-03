using System;
using System.Collections.Generic;

namespace Notifications.DataAccessLayer.RavenClass
{
    public class RavenNotification
    {
        public RavenNotification()
        {
            Receivers = new List<RavenEmployee>();
        }

        public int NotificationId { get; set; }
        public DateTime Date { get; set; }
        public string Content { get; set; }

        public RavenEmployee Sender { get; set; }

        public List<RavenEmployee> Receivers { get; set; }
    }
}