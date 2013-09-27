﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notifications.DataAccessLayer.MongoClass
{
    class MongoMessage
    {
        public string Id { get; set; }
        public DateTime Date { get; set; }
        public string Content { get; set; }

        public string SenderId { get; set; }

        public string ReceiverId { get; set; }
    }
}
