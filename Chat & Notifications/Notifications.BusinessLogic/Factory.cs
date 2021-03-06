﻿using System.Collections.Generic;
using Notifications.Base;

namespace Notifications.BusiessLogic
{
    public class Factory : IFactory
    {
        private readonly IDataRepository _repository;

        public Factory(IDataRepository repository)
        {
            _repository = repository;
        }


        public string AddNotification(INotification notification)
        {
            return _repository.AddNotification(notification);
        }

        public void AddMessage(IMessage message)
        {
            _repository.AddMessage(message);
        }

        public List<INotification> GetReceiveNotifications(string employeeId)
        {
            return _repository.GetReceiveNotifications(employeeId);
        }

        public List<INotification> GetSendNotifications(string employeeId)
        {
            return _repository.GetSendNotifications(employeeId);
        }

        public List<IMessage> GetMessages(string employeeId1, string employeeId2)
        {
            return _repository.GetMessages(employeeId1, employeeId2);
        }



        public void AddTimeofReading(string notificationId, string receiverId)
        {
            _repository.AddTimeofReading(notificationId, receiverId);
        }


        public void AddEmployee(IEmployee employee)
        {
            _repository.AddEmployee(employee);
        }
       
    }
}