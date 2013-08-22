using Notifications.Base;
using System;

namespace Notifications.BusiessLogic
{
    public class Application : IApplication
    {
        private IDataRepository _repository;

        public Application(IDataRepository repository)
        {
            _repository = repository;
        }

        public void BrodcastNotification(string text, int[] recipientsIDs, int senderID)
        {

            INotification notification = new Notification(){ 
                Content= text, 
                Date= DateTime.Now, 
                SenderId= senderID, 
                ReceiversIds= recipientsIDs
            };
           
            _repository.AddNotification(notification);

        }
    }
}