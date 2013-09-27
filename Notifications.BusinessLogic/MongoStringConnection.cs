using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Notifications.Base;

namespace Notifications.BusiessLogic
{
    public class MongoStringConnection: IStringConnection
    {
        public string DatabaseUrl { get; set; }
        public string DatabaseName { get; set; }
    }
}
