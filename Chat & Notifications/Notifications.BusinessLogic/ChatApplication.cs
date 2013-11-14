using System;
using System.Collections.Generic;
using Notifications.Base;

namespace Notifications.BusiessLogic
{
    public class ChatApplication : IChatApplication
    {
        private readonly IFactory _factory;

        public ChatApplication(IFactory factory)
        {
            _factory = factory;
        }

        public string BrodcastNotification(string content, string senderId, List<string> receiversIds, DateTime date)
        {
            INotification notification = new Notification
            {
                Content = content,
                Date = date,
                SenderId = senderId,
                ReceiversIds = receiversIds
            };

           return  _factory.AddNotification(notification);
        }

        public void SendMessage(string content, string senderId, string receiverId, DateTime date)
        {
            IMessage message = new Message
            {
                Content = content,
                SenderId = senderId,
                ReceiverId = receiverId,
                Date = date
            };

            _factory.AddMessage(message);
        }

        public List<INotification> GetReceiveNotifications(string employeeId)
        {
            return _factory.GetReceiveNotifications(employeeId);
        }

        public List<INotification> GetSendNotifications(string employeeId)
        {
            return _factory.GetSendNotifications(employeeId);
        }

        public List<IMessage> GetMessages(string employeeId1, string employeeId2)
        {
            return _factory.GetMessages(employeeId1, employeeId2);
        }

        public void AddTimeofReading(string notificationId, string receiverId)
        {
            _factory.AddTimeofReading(notificationId, receiverId);
        }

        public void AddEmployee(IEmployee employee)
        {
            _factory.AddEmployee(employee);
        }

    }
}