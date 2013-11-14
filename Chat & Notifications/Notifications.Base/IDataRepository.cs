using System.Collections.Generic;

namespace Notifications.Base
{
    public interface IDataRepository
    {
       
        string AddNotification(INotification notification);
        void AddMessage(IMessage message);
        List<INotification> GetReceiveNotifications(string receiverId);
        List<INotification> GetSendNotifications(string senderId);
        List<IMessage> GetMessages(string employeeId1, string employeeId2);

        void AddTimeofReading(string notificationId, string receiverId);
        void AddEmployee(IEmployee employee);
    }
}