using System;
using System.Collections.Generic;
using System.Linq;
using Notifications.Base;
using Notifications.DataAccessLayer.RavenClass;
using Raven.Client.Embedded;
using Raven.Client;
using Notifications.BusiessLogic;

namespace Notifications.DataAccessLayer
{
    public class RavenRepository : IDataRepository
    {

        private EmbeddableDocumentStore documentStore;

        private IDocumentSession _session;

        public RavenRepository()
        {

            
        }



        public void Init()
        {
            try
            {
                documentStore = new EmbeddableDocumentStore
                {

                    DataDirectory = @"App_data\data",

                };

                documentStore.Initialize();

                _session = documentStore.OpenSession();
            }
            catch (Exception e)
            {

            }
        }

        public void AddNotification(INotification notification)
        {
            var sender = _session.Query<RavenEmployee>().FirstOrDefault(x => x.EmployeeId == notification.SenderId);

            var ravenNotification = new RavenNotification
            {
                Sender = sender,
                Date = notification.Date,
                Content = notification.Content,
                
            };

            _session.Store(ravenNotification);
            _session.SaveChanges();

            foreach (var receiverOfNotification in notification.ReceiversIds
                .Select(i => _session.Query<RavenEmployee>()
                .FirstOrDefault(x => x.EmployeeId == i))
                .Select(receiver => new RavenReceiversOfNotification
            {
                Receiver = receiver, 
                Notification = ravenNotification,
                WhenRead = DateTime.Now
            }))
            {
                _session.Store(receiverOfNotification);
                _session.SaveChanges();
            }
        }

        public void AddMessage(IMessage message)
        {
            var sender = _session.Query<RavenEmployee>().FirstOrDefault(x => x.EmployeeId == message.SenderId);
            var recepient = _session.Query<RavenEmployee>().FirstOrDefault(x => x.EmployeeId == message.ReceiverId);

            var ravenMessage = new RavenMessage
            {
                Content = message.Content,
                Date = message.Date,
                Receiver = recepient,
                Sender = sender
            };
            _session.Store(ravenMessage);
            _session.SaveChanges();
        }

        public List<INotification> GetReceiveNotifications(int receiverId)
        {
            List<INotification> notifications = new List<INotification>();
        
            var result = (from item in _session.Query<RavenReceiversOfNotification>()
                where item.Receiver.EmployeeId == receiverId
                select item).ToList();

            foreach (var receiver in result)
            {
                var note =
                    _session.Query<RavenNotification>().FirstOrDefault(x => x.NotificationId == receiver.Notification.NotificationId);
                var employee =
                    _session.Query<RavenEmployee>()
                        .FirstOrDefault(x => x.EmployeeId == receiver.Receiver.EmployeeId);

                Notification n = new Notification()
                {
                    Content = note.Content,
                    Date = note.Date,
                    SenderName = employee.Name
                };
                notifications.Add(n);
            }

            return notifications;
        }

        public List<INotification> GetSendNotifications(int senderId)
        {
            var result = (from notes in _session.Query<RavenNotification>()
                          where notes.Sender.EmployeeId == senderId
                          select new Notification
                          {
                               NotificationId = notes.NotificationId,
                               Content = notes.Content,
                               Date = notes.Date,
                          }).AsEnumerable().Cast<INotification>().ToList();

            foreach (var notes in result)
            {
                notes.ReceiversNames = GetReceivers(notes.NotificationId);
            }

            return result;


        }

        public List<string> GetReceivers(int notesId)
        {
            var result = (from item in _session.Query<RavenReceiversOfNotification>()
                                   where item.Notification.NotificationId == notesId
                                   select item.Receiver.Name).ToList();
            return result;
        }

        public List<IMessage> GetMessages(int employeeId1, int employeeId2)
        {
            var result = (from item in _session.Query<RavenMessage>()
                                     where
                                         ((item.Sender.EmployeeId == employeeId1 && item.Receiver.EmployeeId == employeeId2) ||
                                          (item.Sender.EmployeeId == employeeId2 && item.Receiver.EmployeeId == employeeId1))
                                     orderby item.Date
                                     select new Message
                                     {
                                         Date = item.Date,
                                         Content = item.Content,
                                         SenderName = item.Sender.Name,
                                         ReceiverName = item.Receiver.Name
                                     }).AsEnumerable().Cast<IMessage>().ToList();
            return result;
        }


        public void AddEmployee(IEmployee employee)
        {
            var result = _session.Query<RavenEmployee>().FirstOrDefault(x => x.EmployeeId == employee.EmployeeId);


            if (result == null)
            {
                var newEmployee = new RavenEmployee()
                {
                    Name = employee.Name
                };

                _session.Store(newEmployee);
                _session.SaveChanges();

            }
        }
    }
}
