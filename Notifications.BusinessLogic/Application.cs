using Notifications.Base;

namespace Notifications.BusiessLogic
{
    public class Application : IApplication
    {
        private IDataRepository _repository;

        public Application(IDataRepository repository)
        {
            _repository = repository;
        }

        public void BrodcastNotification(string text, int[] recipientsIDs)
        {
//            foreach (var recipientsID in recipientsIDs)
//            {
//                _repository.addMessage();                
//            }

        }
    }
}