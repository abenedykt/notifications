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
                    ReceiverId = "RavenReceiversOfNotifications/" + receiver,
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
                .Where(x => x.ReceiverId == receiver).ToList();

            var lista = new List<INotification>();

            foreach (var item in result)
            {
                var note = new Notification()
                {
                    Content = _session.Load<RavenNotification>(item.NotificationId).Content,
                    Date = _session.Load<RavenNotification>(item.NotificationId).Date,
                    SenderName = _session.Load<RavenEmployee>(item.ReceiverId).Name
                };
                lista.Add(note);
            }

            return lista;

            
            //var result = (from item in _session.Query<RavenReceiversOfNotification>()
            //              from employee in _session.Query<RavenEmployee>()
            //              from note in _session.Query<RavenNotification>()
            //    where item.ReceiverId ==  String.Format("RavenEmployees/{0}",receiverId)
            //    select new Notification()
            //    {
            //        Content = note.Content,
            //        Date = note.Date,
            //        SenderName = employee.Name
            //    }).AsEnumerable().Cast<INotification>().ToList();


            //return result;
        }

        public List<INotification> GetSendNotifications(int senderId)
        {

            var result = (from item in _session.Query<RavenNotification>()
                          where item.SenderId == String.Format("RavenEmployees/{0}", senderId)
                          select item).ToList();
            
            var lista = new List<INotification>();
           
            foreach (var notes in result)
            {
                var notifica = new Notification
                {
                    Content = notes.Content,
                    Date = notes.Date,
                    ReceiversNames = GetReceivers(notes.Id)
                };
                lista.Add(notifica);
            }           
            return lista;
        }

        public List<string> GetReceivers(string notesId)
        {
            var result = _session.Query<RavenReceiversOfNotification>()
                .Customize(x => x.Include<RavenReceiversOfNotification>(o => o.ReceiverId))
                .Where(x => x.NotificationId == notesId)
                .ToList();

            var l = new List<string>();

            foreach (var name in result)
            { 
                l.Add(_session.Load<RavenEmployee>(name.ReceiverId).Name);      
            }
            return l;
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

            var lista = new List<IMessage>();

            foreach (var message in result)
            {
                var m = new Message
                {
                    Content = message.Content,
                    Date = message.Date,
                    ReceiverName = _session.Load<RavenEmployee>(message.ReceiverId).Name,
                    SenderName = _session.Load<RavenEmployee>(message.SenderId).Name
                };
                
                lista.Add(m);


            }

            return lista;


        //    var result = (from item in _session.Query<RavenMessage>()
        //                  from employee in _session.Query<RavenEmployee>()
        //                             where
        //                                 ((item.SenderId == employee1 && item.ReceiverId == employee2) ||
        //                                  (item.SenderId == employee2 && item.ReceiverId == employee1))
        //                             orderby item.Date
        //                             select new Message
        //                             {
        //                                 Date = item.Date,
        //                                 Content = item.Content,
        //                                 SenderName = employee.Name,
        //                                 ReceiverName = employee.Name
        //                             }).AsEnumerable().Cast<IMessage>().ToList();
        //    return result;
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
            documentStore.Conventions.RegisterIdConvention<RavenEmployee>((dbname, commands, user) => "RavenEmployees/" + user.EmployeeId);
            _session.Store(newEmployee);
            _session.SaveChanges();
        }
    }
}
