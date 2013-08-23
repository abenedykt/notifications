using Notifications.Base;
using System;
using System.Collections.Generic;

namespace Notifications.BusiessLogic
{
    public class Application : IApplication
    {
        private IDataRepository _repository;

        public Application(IDataRepository repository)
        {
            _repository = repository;
        }

        public void BrodcastNotification(string content, int senderId, List<int> recipientsIds, DateTime date)
        {

            INotification notification = new Notification(){ 
                Content= content, 
                Date= date, 
                SenderId= senderId, 
                ReceiversIds= recipientsIds
            };
           
            _repository.AddNotification(notification);

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
            _repository.AddMessage(message);

        }

        public List<IMessage> GetMessages(int employeeId1, int employeeId2)
        {
            return _repository.GetMessages(employeeId1, employeeId2);
        }
    }
}