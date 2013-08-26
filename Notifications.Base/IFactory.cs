using System.Collections.Generic;
namespace Notifications.Base
{
    public interface IFactory
    {
        void AddNotification(INotification notification);

        void AddMessage(IMessage message);
        List<INotification> GetReceiveNotifications(int employeeId);
        List<INotification> GetSendNotifications(int employeeId);
        List<IMessage> GetMessages(int employeeId1, int employeeId2);
    }
}