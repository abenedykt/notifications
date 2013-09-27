using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Notifications.Base;
using Notifications.BusiessLogic;
using Notifications.DataAccessLayer;
using Raven.Client;
using Raven.Client.Document;
using Xunit.Extensions;

namespace ExpensiveTests
{
    public class ExpensiveTests
    {
        private readonly RavenRepository _ravenRepository =
            new RavenRepository(new RavenStringConnection
            {
                DatabaseUrl = "localhost:8080",
                DatabaseName = "chat"
            });
        private readonly ChatApplication _application = new ChatApplication(new Factory(new RavenRepository(new RavenStringConnection
            {
                DatabaseUrl = "localhost:8080",
                DatabaseName = "chat"
            })));

        private DocumentStore _documentStore;

        private IDocumentSession _session;

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
            List<INotification> result = _ravenRepository.GetReceiveNotifications(receiverId);
            int items = result.Count;
        }

        [Theory]
        [InlineData(2)]
        public void GetSendNotifications_ExpensiveTest(int senderId)
        {
            List<INotification> result = _ravenRepository.GetSendNotifications(senderId);

            int items = result.Count;
        }

        [Theory]
        [InlineData(2203, 292462)]
        public void GetMessages_ExpensiveTest(int senderId, int receiverId)
        {
            List<IMessage> result = _ravenRepository.GetMessages(senderId, receiverId);

            int items = result.Count;
        }

        [Theory]
        [InlineData(5313, 3)]
        public void AddTimeOfReading_ExpensiveTest(int notificationId, int receiverId)
        {
            _ravenRepository.AddTimeofReading(notificationId, receiverId);

            var result = _ravenRepository.GetReceiveNotifications(receiverId);
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
        [InlineData("notatka", 9, new[] { 1, 2, 3 })]
        public void AddNotification_ExpensiveTest(string content, int senderId, int[] receiversIds)
        {
            List<int> lista = receiversIds.ToList();

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
        [InlineData("notatka", 9, new[] { 3, 5, 6 })]
        public void AddNotificationsAndMeasureTime_ExpensiveTest(string content, int senderId, int[] receiversIds)
        {
            List<int> receivers = receiversIds.ToList();
            var date = new DateTime(2013, 9, 10, 12, 12, 12);

            var watch = Stopwatch.StartNew();

            for (int i = 0; i < 250; i++)
                _application.BrodcastNotification(content, senderId, receivers, date);

            watch.Stop();

            var n = watch.ElapsedMilliseconds;
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
        [InlineData("notatka", 10, 7)]
        public void AddMessagesAndMeasureTime_ExpensiveTest(string content, int senderId, int receiverId)
        {
            var date = new DateTime(2013, 9, 10, 12, 12, 12);

            var watch = Stopwatch.StartNew();

            for (int i = 0; i < 100000; i++)
            {
                _application.SendMessage(content, senderId, receiverId, date);
            }

            watch.Stop();

            var n = watch.ElapsedMilliseconds;
        }
    }
}