using System;
using System.Collections.Generic;

namespace Notifications.Base
{
    public interface INotification
    {
        int SenderId { get; set; }
        DateTime Date { get; set; }
        string Content { get; set; }
        List<int> ReceiversIds { get; set; }
    }
}