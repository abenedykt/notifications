using System;

namespace Notifications.Base
{
    public interface IMessage
    {
        string Content { get; set; }
        DateTime Date { get; set; }
        string SenderId { get; set; }
        string SenderName { get; set; }
        string ReceiverId { get; set; }
        string ReceiverName { get; set; }
    }
}