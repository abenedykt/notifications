using System;
using Notifications.Base;

namespace Notifications.BusiessLogic
{

    public class Message : IMessage
    {
        
        public string Content { get; set; }
        public DateTime Date { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
       
    }



}
