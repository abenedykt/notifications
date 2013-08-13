using DataAccessLayer;
using System.Collections.Generic;

namespace BusinessLogic
{
    public class Factory
    {
        public void addNotification(NotificationClass notification)
        {
            var note = new Notification();
            note.Date = notification.Date;
            note.Content = notification.Content;
            note.SenderId = notification.SenderId;

            var repo = new SQLRepository();
            repo.addNotification(note);
        }

        public void addMessage(MessageClass message)
        {
          
        }

        //public List<NotificationClass> getReceiveNotifications();
        //public List<NotificationClass> getSendNotifications();
        //public List<NotificationClass> getMessages();
    }
}