using System;
using System.Collections.Generic;
using Notifications.Base;

namespace Notifications.BusiessLogic
{
    public class Application : IApplication
    {
        private readonly IFactory _factory;

        public Application(IFactory factory)
        {
            _factory = factory;
        }

        public int BrodcastNotification(string content, int senderId, List<int> receiversIds, DateTime date)
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

        public void SendMessage(string content, int senderId, int receiverId, DateTime date)
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

        public List<INotification> GetReceiveNotifications(int employeeId)
        {
            return _factory.GetReceiveNotifications(employeeId);
        }

        public List<INotification> GetSendNotifications(int employeeId)
        {
            return _factory.GetSendNotifications(employeeId);
        }

        public List<IMessage> GetMessages(int employeeId1, int employeeId2)
        {
            return _factory.GetMessages(employeeId1, employeeId2);
        }

        public void AddTimeofReading(int notificationId, int receiverId)
        {
            _factory.AddTimeofReading(notificationId, receiverId);
        }

        public void AddEmployee(IEmployee employee)
        {
            _factory.AddEmployee(employee);
        }

    }
}