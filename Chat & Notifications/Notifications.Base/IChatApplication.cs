using System;
using System.Collections.Generic;

namespace Notifications.Base
{
    public interface IChatApplication
    {
        string BrodcastNotification(string content, string senderId, List<string> receiversIds, DateTime date);
        void SendMessage(string content, string senderId, string receiverId, DateTime date);
        List<INotification> GetReceiveNotifications(string employeeId);
        List<INotification> GetSendNotifications(string employeeId);
        List<IMessage> GetMessages(string employeeId1, string employeeId2);
        void AddTimeofReading(string notificationId, string receiverId);
        void AddEmployee(IEmployee employee);
    }
}