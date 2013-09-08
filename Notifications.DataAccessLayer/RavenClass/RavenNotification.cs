using System;
using System.Collections.Generic;

namespace Notifications.DataAccessLayer.RavenClass
{
    public class RavenNotification
    {
        public string Id { get; set; }
        public DateTime Date { get; set; }
        public string Content { get; set; }

        public string SenderId { get; set; }
    }
}