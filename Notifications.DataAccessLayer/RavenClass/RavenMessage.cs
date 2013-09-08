using System;

namespace Notifications.DataAccessLayer.RavenClass
{
    public class RavenMessage
    {
        public string Id { get; set; }
        public DateTime Date { get; set; }
        public string Content { get; set; }

        public string SenderId { get; set; }

        public string ReceiverId { get; set; }
    }
}