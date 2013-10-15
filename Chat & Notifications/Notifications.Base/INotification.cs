using System;
using System.Collections.Generic;

namespace Notifications.Base
{
    public interface INotification
    {
        int NotificationId { get; set; }
        int SenderId { get; set; }
        string SenderName { get; set; }
        DateTime Date { get; set; }
        string Content { get; set; }
        List<int> ReceiversIds { get; set; }
        List<string> ReceiversNames { get; set; }
      
    }
}