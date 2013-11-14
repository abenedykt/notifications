using System.Collections.Generic;

namespace Notifications.Base
{
    public interface IFactory
    {
        string AddNotification(INotification notification);
        void AddMessage(IMessage message);
        List<INotification> GetReceiveNotifications(string employeeId);
        List<INotification> GetSendNotifications(string employeeId);
        List<IMessage> GetMessages(string employeeId1, string employeeId2);
        void AddTimeofReading(string notificationId, string receiverId);
        void AddEmployee(IEmployee employee);
    }
}