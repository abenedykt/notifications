using System;
using System.Collections.Generic;
namespace Notifications.Base
{
    public interface IApplication
    {
        void BrodcastNotification(string content, int senderId, List<int> recipientsIds, DateTime date);
        void SendMessage(string content, int senderId, int receiverId, DateTime date);
        List<IMessage> GetMessages(int employeeId1, int employeeId2);

    }
}