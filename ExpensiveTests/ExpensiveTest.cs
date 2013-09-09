using System.Linq;
using Notifications.DataAccessLayer;
using Notifications.BusiessLogic;
using Raven.Client;
using Raven.Client.Document;
using Xunit.Extensions;
using Notifications.Base;
using System;

namespace ExpensiveTests
{
    public class ExpensiveTests
    {
        private DocumentStore _documentStore;

        private IDocumentSession _session;

        private readonly RavenRepository _ravenRepository = new RavenRepository();

       // void Init()
        //{
        //    _documentStore = new DocumentStore
        //    {
        //        Url = "http://localhost:8080"
        //    };

        //    _documentStore.Initialize();

        //    _session = _documentStore.OpenSession();
        //}

        [Theory]
        [InlineData(1)]
        public void GetReceiveNotifications_ExpensiveTest(int receiverId)
        {    
           var result = _ravenRepository.GetReceiveNotifications(receiverId);  
           var items = result.Count;
        }

        [Theory]
        [InlineData(2)]
        public void GetSendNotifications_ExpensiveTest(int senderId)
        {
            var result = _ravenRepository.GetSendNotifications(senderId);

            var items = result.Count;
        }

        [Theory]
        [InlineData("Adam5001", 5001)]
        public void AddEmployee_ExpensiveTest(string name, int employeeId)
        {
            var employee = new Employee
            {
                Name = name,
                EmployeeId = employeeId
            };

           _ravenRepository.AddEmployee(employee);
        }

        [Theory]
        [InlineData("notatka", 10, new int[]{3, 5, 6})]
        public void AddNotification_ExpensiveTest(string content, int senderId, int[] receiversIds)
        {
            var lista = receiversIds.ToList();

            INotification notification = new Notification
            {
                Content = content,
                Date = DateTime.Now,
                SenderId = senderId,
                ReceiversIds = lista
            };

            _ravenRepository.AddNotification(notification);
        }

        [Theory]
        [InlineData("notatka", 10, 7)]
        public void AddMessage_ExpensiveTest(string content, int senderId, int receiverId)
        {           
            IMessage message = new Message
            {
                Content = content,
                SenderId = senderId,
                ReceiverId = receiverId,
                Date = DateTime.Now
            };

            _ravenRepository.AddMessage(message);
        }

        [Theory]
        [InlineData(3012,2244)]
        public void GetMessages_ExpensiveTest(int senderId, int receiverId)
        {
            var result= _ravenRepository.GetMessages(senderId, receiverId);

            var items = result.Count;
        }
    }
}
