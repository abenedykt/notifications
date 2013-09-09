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

        private readonly DocumentStore _documentStore;

        private readonly IDocumentSession _session;

        public RavenRepository()
        {
            try
            {
                _documentStore = new DocumentStore
                {
                    Url = "http://localhost:8080"
                };

                _documentStore.Initialize();
                
                _session = _documentStore.OpenSession();
                _session.Advanced.MaxNumberOfRequestsPerSession = 1000;
            }
            catch (Exception e)
            {        
                Debug.WriteLine(e.Message);
            }
        }
    
        public void AddNotification(INotification notification)
        {
            var ravenNotification = new RavenNotification
            {
                SenderId = "RavenEmployees/" + notification.SenderId,
                Date = notification.Date,
                Content = notification.Content,              
            };

            _session.Store(ravenNotification);
            _session.SaveChanges();

            foreach (var receiver in notification.ReceiversIds)
            {
                var receiverOfNotification = new RavenReceiversOfNotification()
                {
                    ReceiverId = "RavenEmployees/" + receiver,
                    NotificationId = ravenNotification.Id,
                    WhenRead = DateTime.Now
                };

                _session.Store(receiverOfNotification);
                _session.SaveChanges();
            }
        }

        public void AddMessage(IMessage message)
        {
            var ravenMessage = new RavenMessage
            {
                Content = message.Content,
                Date = message.Date,
                ReceiverId = "RavenEmployees/" + message.ReceiverId,
                SenderId = "RavenEmployees/" + message.SenderId
            };
            _session.Store(ravenMessage);
            _session.SaveChanges();
        }

        public List<INotification> GetReceiveNotifications(int receiverId)
        {
            var receiver =  String.Format("RavenEmployees/{0}",receiverId);

            var result = _session.Query<RavenReceiversOfNotification>()
                .Customize(x => x.Include<RavenReceiversOfNotification>(o => o.ReceiverId))
                .Customize(x => x.Include<RavenReceiversOfNotification>(o => o.NotificationId))
                .Where(x => x.ReceiverId == receiver).OrderByDescending(x=>x.Date).Take(30).ToList();

            return result.Select(item => new Notification()
            {
                Content = _session.Load<RavenNotification>(item.NotificationId).Content, 
                Date = _session.Load<RavenNotification>(item.NotificationId).Date, 
                SenderName = _session.Load<RavenEmployee>(_session.Load<RavenNotification>(item.NotificationId).SenderId).Name
            }).AsEnumerable().Cast<INotification>().ToList();

        }

        public List<INotification> GetSendNotifications(int senderId)
        {
            var sender = String.Format("RavenEmployees/{0}", senderId);

            var result = _session.Query<RavenNotification>().Where(x => x.SenderId == sender).OrderByDescending(x => x.Date).Take(30).ToList();

            return result.Select(notes => new Notification
                {
                    Content = notes.Content,
                    Date = notes.Date,
                    ReceiversNames = GetReceivers(notes.Id)
                }).AsEnumerable().Cast<INotification>().ToList();
        }

        public List<string> GetReceivers(string notesId)
        {
            var result = _session.Query<RavenReceiversOfNotification>()
                .Customize(x => x.Include<RavenReceiversOfNotification>(o => o.ReceiverId))
                .Where(x => x.NotificationId == notesId)
                .ToList();

            return result.Select(name => _session.Load<RavenEmployee>(name.ReceiverId).Name).ToList();
        }

        public List<IMessage> GetMessages(int employeeId1, int employeeId2)
        {
            var employee1 = String.Format("RavenEmployees/{0}", employeeId1);
            var employee2 = String.Format("RavenEmployees/{0}", employeeId2);

            var result = _session.Query<RavenMessage>()
                .Customize(x => x.Include<RavenMessage>(o => o.ReceiverId))
                .Customize(x => x.Include<RavenMessage>(o => o.SenderId))
                .Where((x =>( x.ReceiverId == employee1 && x.SenderId == employee2) || (x.ReceiverId == employee2 && x.SenderId == employee1)))
                .ToList();

            return result.Select(message => new Message
                {
                    Content = message.Content,
                    Date = message.Date,
                    ReceiverName = _session.Load<RavenEmployee>(message.ReceiverId).Name,
                    SenderName = _session.Load<RavenEmployee>(message.SenderId).Name
                }).AsEnumerable().Cast<IMessage>().ToList();
        }


        public void AddEmployee(IEmployee employee)
        {         
            var result = _session.Query<RavenEmployee>().FirstOrDefault(x => x.EmployeeId == employee.EmployeeId);

            if (result != null) return;

            var newEmployee = new RavenEmployee()
            {
                EmployeeId = employee.EmployeeId,
                Name = employee.Name
            };
            _documentStore.Conventions.RegisterIdConvention<RavenEmployee>((dbname, commands, user) => "RavenEmployees/" + user.EmployeeId);
            _session.Store(newEmployee);
            _session.SaveChanges();
        }
    }
}
