using System;
using System.Collections.Generic;
using Notifications.Base;

namespace Notifications.BusiessLogic
{
   
    
    public class Notification : INotification
    {
        public int NotificationId { get; set; }
        public int SenderId { get; set; }
        public string SenderName { get; set; }
        public DateTime Date { get; set; }
        public string Content { get; set; }
        public List<int> ReceiversIds { get; set; }
        public List<string> ReceiversNames { get; set; }
  
        //public void AddNotification(DateTime date,int senderId,string content, List<int> receiversId)
        //{
        //    Date = date;
        //    SenderId = senderId;
        //    Content = content;
        //    ReceiversIds = receiversId;

        //    var factory = new Factory();
        //    factory.AddNotification(this);
        //}


    }


    
}