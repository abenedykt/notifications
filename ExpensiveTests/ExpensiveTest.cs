using System.Linq;
using Notifications.DataAccessLayer;
using Notifications.DataAccessLayer.RavenClass;
using Notifications.BusiessLogic;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Linq;
using Xunit;
using Xunit.Extensions;
using Notifications.Base;
using System;
using System.Collections.Generic;

namespace ExpensiveTests
{
    public class ExpensiveTests
    {
        private DocumentStore documentStore;

        private IDocumentSession _session;

        private RavenRepository _ravenRepository = new RavenRepository();

        void Init()
        {
            documentStore = new DocumentStore
            {
                Url = "http://localhost:8080"
            };

            documentStore.Initialize();

            _session = documentStore.OpenSession();
        }

        [Theory]
        [InlineData(680)]
        public void GetReceiveNotifications_ExpensiveTest(int receiverId)
        {    
           var result = _ravenRepository.GetReceiveNotifications(receiverId);  
           var items = result.Count;
        }

        [Theory]
        [InlineData(4231)]
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
            var lista = new List<int>();

            foreach (var item in receiversIds)
            {
                lista.Add(item);                
            }

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
        [InlineData(4047,2156)]
        public void GetMessages_ExpensiveTest(int senderId, int receiverId)
        {
            var result= _ravenRepository.GetMessages(senderId, receiverId);

            var items = result.Count;
        }
    }
}
