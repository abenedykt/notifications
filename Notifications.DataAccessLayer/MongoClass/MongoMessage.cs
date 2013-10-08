﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Notifications.DataAccessLayer.MongoClass
{
    class MongoMessage
    {
        public ObjectId Id { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime Date { get; set; }
        public string Content { get; set; }

        public int SenderId { get; set; }

        public int ReceiverId { get; set; }
    }
}
