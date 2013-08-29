﻿using Notifications.Base;
using System;
using System.Collections.Generic;

namespace Notifications.BusiessLogic
{
    public class Application : IApplication
    {
        private IFactory _factory;

        public Application(IFactory factory)
        {
            _factory = factory;
        }

        public void BrodcastNotification(string content, int senderId, List<int> receiversIds, DateTime date)
        {
            INotification notification = new Notification(){ 
                Content= content, 
                Date= date, 
                SenderId= senderId,
                ReceiversIds = receiversIds
            };
           
            _factory.AddNotification(notification);
        }

        public void SendMessage(string content, int senderId, int receiverId, DateTime date)
        {
            IMessage message = new Message()
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


       



    }
}