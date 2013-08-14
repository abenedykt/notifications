using DataAccessLayer;
using System;
using System.Collections.Generic;


namespace BusinessLogic
{
   
    
    public class NotificationClass
    {
        public DateTime Date;
        public int SenderId;
        public string Content;
        public List<int> ReceiversId;

  
        public void addNotification(DateTime date,int senderId,string content, List<int> receiversId)
        {
            Date = date;
            SenderId = senderId;
            Content = content;
            ReceiversId = receiversId;

            var factory = new Factory();
            factory.addNotification(this);
        }


    }
}