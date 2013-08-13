using System.Collections.Generic;
namespace DataAccessLayer
{
    public interface IDataRepository
    {


        void addNotification(Notification notification);
        void addMessage(Message message);
        List<Notification> getReceiveNotifications(int ReceiverId);
        List<Notification> getSendNotifications(int SenderId);
        List<Message> getMessages(int EmployeeId1, int EmployeeId2);

     }
}