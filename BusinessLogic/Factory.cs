using DataAccessLayer;
using System.Collections.Generic;

namespace BusinessLogic
{
    public class Factory
    {

        public IDataRepository getRepository()
        {
            return new SQLRepository();

        }

        public void addNotification(NotificationClass notification)
        {
           
        }

        
        //public void addMessage(MessageClass message);
        //public List<NotificationClass> getReceiveNotifications();
        //public List<NotificationClass> getSendNotifications();
        //public List<NotificationClass> getMessages();
    }
}