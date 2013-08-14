using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic
{

    public class MessageTest
    {
    }

    public class MessageClass
    {
        int SenderId;
        int ReceiverId;
        DateTime Date;
        string Content;


        public void addMessage()
        {
            var factory = new Factory();
            //factory.addMessage(this);
        }


    }
}
