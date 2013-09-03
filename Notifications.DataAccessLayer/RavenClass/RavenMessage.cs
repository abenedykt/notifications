using System;

namespace Notifications.DataAccessLayer.RavenClass
{
    public class RavenMessage
    {
        public int MessageId { get; set; }
        public DateTime Date { get; set; }
        public string Content { get; set; }

        public RavenEmployee Sender { get; set; }

        public RavenEmployee Receiver { get; set; }
    }
}