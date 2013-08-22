using System;

namespace Notifications.Base
{
    public interface INotification
    {
        int SenderId { get; set; }
        DateTime Date { get; set; }
        string Content { get; set; }
        int[] ReceiversIds { get; set; }
    }
}