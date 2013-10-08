using System;
<<<<<<< HEAD
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
=======
>>>>>>> f815d6b6916713a298d17f452d21f57cc62eb8cf

namespace Notifications.DataAccessLayer.MongoClass
{
    class MongoNotification
    {
        public ObjectId Id { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime Date { get; set; }
        public string Content { get; set; }

        public int SenderId { get; set; }
    }
}
