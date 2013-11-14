using System;
using MongoDB.Bson;

namespace Notifications.DataAccessLayer.MongoClass
{
    class MongoReceiversOfNotification
    {
        public ObjectId Id { get; set; }
        public string NotificationId { get; set; }
        public string ReceiverId { get; set; }

        public DateTime Date { get; set; }
        public DateTime WhenRead { get; set; }
    }
}
