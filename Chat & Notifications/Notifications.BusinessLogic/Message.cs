using System;
using Notifications.Base;

namespace Notifications.BusiessLogic
{
    public class Message : IMessage
    {
        public string Content { get; set; }
        public DateTime Date { get; set; }
        public string SenderId { get; set; }
        public string SenderName { get; set; }
        public string ReceiverId { get; set; }
        public string ReceiverName { get; set; }
    }
}