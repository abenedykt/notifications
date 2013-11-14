using System;
using System.Collections.Generic;

namespace Notifications.Base
{
    public interface INotification
    {
        string NotificationId { get; set; }
        string SenderId { get; set; }
        string SenderName { get; set; }
        DateTime Date { get; set; }
        string Content { get; set; }
        List<string> ReceiversIds { get; set; }
        List<string> ReceiversNames { get; set; }
      
    }
}