using Notifications.Base;
using System.Collections.Generic;

namespace Notifications.BusiessLogic
{
    public class Factory : IFactory
    {
        private IDataRepository _repository;

        public Factory(IDataRepository repository)
        {
            _repository = repository;
        }


        public void AddNotification(INotification notification)
        {
            _repository.AddNotification(notification);
        }
        public void AddMessage(IMessage message)
        {
            _repository.AddMessage(message);
        }

        public List<INotification> GetReceiveNotifications(int employeeId)
        {
            return _repository.GetReceiveNotifications(employeeId);
        }
        public List<INotification> GetSendNotifications(int employeeId)
        {
            return _repository.GetSendNotifications(employeeId);
        }

        public List<IMessage> GetMessages(int employeeId1, int employeeId2)
        {
            return _repository.GetMessages(employeeId1, employeeId2);
        }
    }
}