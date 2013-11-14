using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Notifications.DataAccessLayer.MongoClass
{
    class MongoNotification
    {
        public ObjectId Id { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime Date { get; set; }
        public string Content { get; set; }

        public string SenderId { get; set; }
    }
}
