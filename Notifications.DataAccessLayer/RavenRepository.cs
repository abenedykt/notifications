using System;
using System.Collections.Generic;
using System.Data.Metadata.Edm;
using System.Diagnostics;
using System.Linq;
using Notifications.Base;
using Notifications.DataAccessLayer.RavenClass;
using Raven.Client.Document;
using Raven.Client;
using Notifications.BusiessLogic;

namespace Notifications.DataAccessLayer
{
    public class RavenRepository : IDataRepository
    {

        private DocumentStore documentStore;

        private IDocumentSession _session;

        public RavenRepository()
        {
            try
            {
                documentStore = new DocumentStore
                {
                    Url = "http://localhost:8080"
                };

                documentStore.Initialize();

                _session = documentStore.OpenSession();
            }
            catch (Exception e)
            {        
                Debug.WriteLine(e.Message);
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
              receiverOfNotification.Receiver.ReceiveNotifications.Add(receiverOfNotification.Notification); 
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
            
            
            var result = (from item in _session.Query<RavenReceiversOfNotification>()
                where item.Receiver.EmployeeId == receiverId
                select new Notification()
                {
                    Content = item.Notification.Content,
                    Date = item.Notification.Date,
                    SenderName = item.Receiver.Name
                }).AsEnumerable().Cast<INotification>().ToList();


            return result;
        }

        public List<INotification> GetSendNotifications(int senderId)
        {

            var result = (from item in _session.Query<RavenNotification>()
               // where (item.Sender == null)
                orderby item.NotificationId
                //where item.NotificationId == 1002
                select item).ToList();
            var items = result.Count;
            return null;
            //foreach (var notes in result)
            //{
            //    notes.ReceiversNames = GetReceivers(notes.NotificationId);
            //}

            //return result;

            // var result = (from item in _session.Query<RavenNotification>()
            //             select item).ToList();

            //return new List<INotification>();
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

            if (result != null) return;

            var newEmployee = new RavenEmployee
            {
                EmployeeId = employee.EmployeeId,
                Name = employee.Name
            };

            _session.Store(newEmployee);
            _session.SaveChanges();
        }
    }
}
