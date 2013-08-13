using System.Collections.Generic;
using System.Linq;

namespace DataAccessLayer
{
    public class SQLRepository : IDataRepository
    {
        ContextNotifications context = new ContextNotifications();


        public void addNotification(Notification notification)
        {
            using(context)
            {
                context.Notifications.Add(notification);
                context.SaveChanges();

            }
        }

        public void addMessage(Message message)
        {
            using(context)
            {
                context.Messages.Add(message);
                context.SaveChanges();
            }
        }

        public List<Notification> getReceiveNotifications(int ReceiverId)
        {
            using (context)
            {
                var result = (from item in context.ReceiversOfNotification
                              join note in context.Notifications on item.ReceiveNotificationId equals note.NotificationId
                              where item.ReceiverId == ReceiverId
                              select note).ToList();
                return result;
            }  
        }

        public List<Notification> getSendNotifications(int SenderId)
        {
            using (context)
            {
                var result = (from item in context.Notifications
                              where item.SenderId == SenderId
                              select item).ToList();
                return result;
            }                 
        }


        public List<Message> getMessages(int EmployeeId1, int EmployeeId2)
        {
            using (context)
            {
                var result = (from item in context.Messages
                              where ((item.SenderId == EmployeeId1 && item.ReceiverId == EmployeeId2) ||( item.SenderId == EmployeeId2 && item.ReceiverId == EmployeeId1))
                              select item).ToList();
                return result;
            }  
        }
    }
}