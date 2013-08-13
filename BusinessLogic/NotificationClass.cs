using DataAccessLayer;
using System;
using Xunit;
using Xunit.Extensions;

namespace BusinessLogic
{
    public class NotificationTest
    {
       public void addNotificationTest()
        {
            var note = new NotificationClass();
        }
    }
    
    public class NotificationClass
    {
        public DateTime Date;
        public int SenderId;
        public string Content;
        public int[] ReceiversId;

  
        public void addNotification(DateTime date,int senderId,string content)
        {
            Date = date;
            SenderId = senderId;
            Content = content;

            var factory = new Factory();
            factory.addNotification(this);
        }


    }
}