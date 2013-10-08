<<<<<<< HEAD
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace Notifications.DataAccessLayer.MongoClass
=======
﻿namespace Notifications.DataAccessLayer.MongoClass
>>>>>>> f815d6b6916713a298d17f452d21f57cc62eb8cf
{
    class MongoEmployee
    {
        public ObjectId  Id { get; set; }
        public int EmployeeId { get; set; }
        public string Name { get; set; }
    }
}
