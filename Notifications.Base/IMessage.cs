using System;

namespace Notifications.Base
{
    public interface IMessage
    {
        string Content { get; set; }
        DateTime Date { get; set; }
        int SenderId { get; set; }
        int ReceiverId { get; set; }
    }
}
