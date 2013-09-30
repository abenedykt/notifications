using System;

namespace Notifications.DataAccessLayer.MongoClass
{
    class MongoNotification
    {
        public string Id { get; set; }
        public DateTime Date { get; set; }
        public string Content { get; set; }

        public string SenderId { get; set; }
    }
}
