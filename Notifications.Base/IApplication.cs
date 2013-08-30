using System;
using System.Collections.Generic;

namespace Notifications.Base
{
    public interface IApplication
    {
        void BrodcastNotification(string content, int senderId, List<int> receiversIds, DateTime date);
        void SendMessage(string content, int senderId, int receiverId, DateTime date);
        List<INotification> GetReceiveNotifications(int employeeId);
        List<INotification> GetSendNotifications(int employeeId);
        List<IMessage> GetMessages(int employeeId1, int employeeId2);
    }
}