using System.Collections.Generic;

namespace Notifications.Base
{
    public interface IDataRepository
    {
        int AddNotification(INotification notification);
        void AddMessage(IMessage message);
        List<INotification> GetReceiveNotifications(int receiverId);
        List<INotification> GetSendNotifications(int senderId);
        List<IMessage> GetMessages(int employeeId1, int employeeId2);

        void AddTimeofReading(int notificationId, int receiverId);
        void AddEmployee(IEmployee employee);
    }
}