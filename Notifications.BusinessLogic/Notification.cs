using System;
using System.Collections.Generic;
using Notifications.Base;

namespace Notifications.BusiessLogic
{
   
    
    public class Notification : INotification
    {
        public List<int> ReceiversId;

  
        public void AddNotification(DateTime date,int senderId,string content, List<int> receiversId)
        {
            Date = date;
            SenderId = senderId;
            Content = content;
            ReceiversId = receiversId;

            var factory = new Factory();
            factory.AddNotification(this);
        }


        public int SenderId { get; set; }
        public DateTime Date { get; set; }
        public string Content { get; set; }
        public int[] ReceiversIds { get; set; }
    }


    
}