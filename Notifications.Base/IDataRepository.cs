using System.Collections.Generic;

namespace Notifications.Base
{
    public interface IDataRepository
    {
        List<IEmployee> GetLogin();
        void AddNotification(INotification notification);
        void AddMessage(IMessage message);
        List<INotification> GetReceiveNotifications(int receiverId);
        List<INotification> GetSendNotifications(int senderId);
        List<IMessage> GetMessages(int employeeId1, int employeeId2);
     }
}