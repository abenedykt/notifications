using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Notifications.Base;
using Notifications.BusiessLogic;
using Notifications.DataAccessLayer.RavenClass;
using Raven.Client;
using Raven.Client.Document;

namespace Notifications.DataAccessLayer
{
    public class RavenRepository : IDataRepository
    {
        private readonly DocumentStore _documentStore;

        public RavenRepository(RavenStringConnection ravenConnection)
        {
            try
            {
                _documentStore = new DocumentStore
                {
                    Url = ravenConnection.DatabaseUrl,
                    DefaultDatabase = ravenConnection.DatabaseName
                };
                _documentStore.Initialize();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        public int AddNotification(INotification notification)
        {
            var ravenNotification = new RavenNotification
            {
                SenderId = "RavenEmployees/" + notification.SenderId,
                Date = notification.Date,
                Content = notification.Content,
            };
            using (IDocumentSession session = _documentStore.OpenSession())
            {
                session.Store(ravenNotification);
                session.SaveChanges();
            }


            foreach (int receiver in notification.ReceiversIds)
            {
                var receiverOfNotification = new RavenReceiversOfNotification
                {
                    ReceiverId = "RavenEmployees/" + receiver,
                    NotificationId = ravenNotification.Id,
                    Date = ravenNotification.Date
                };

                using (IDocumentSession session = _documentStore.OpenSession())
                {
                    session.Store(receiverOfNotification);
                    session.SaveChanges();
                }
            }

            var s = new string((ravenNotification.Id).Where(Char.IsNumber).ToArray());
            int cs = Convert.ToInt32(s);

            return cs;
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

            using (IDocumentSession session = _documentStore.OpenSession())
            {
                session.Store(ravenMessage);
                session.SaveChanges();
            }
        }

        public List<INotification> GetReceiveNotifications(int receiverId)
        {
            var receiver = String.Format("RavenEmployees/{0}", receiverId);

            using (IDocumentSession session = _documentStore.OpenSession())
            {
                session.Advanced.MaxNumberOfRequestsPerSession = 1000;

                var result = session.Query<RavenReceiversOfNotification>()
                    .Customize(x => x.Include<RavenReceiversOfNotification>(o => o.ReceiverId))
                    .Customize(x => x.Include<RavenReceiversOfNotification>(o => o.NotificationId))
                    .Where(x => x.ReceiverId == receiver).OrderByDescending(x => x.Date).Take(30).ToList();

                return result.Select(item => new Notification
                {
                    Content = session.Load<RavenNotification>(item.NotificationId).Content,
                    Date = session.Load<RavenNotification>(item.NotificationId).Date,
                    SenderName =
                        session.Load<RavenEmployee>(session.Load<RavenNotification>(item.NotificationId).SenderId)
                            .Name
                }).AsEnumerable().Cast<INotification>().ToList();
            }
        }

        public List<INotification> GetSendNotifications(int senderId)
        {
            var sender = String.Format("RavenEmployees/{0}", senderId);

            using (var session = _documentStore.OpenSession())
            {
                session.Advanced.MaxNumberOfRequestsPerSession = 1000;

                var result =
                    session.Query<RavenNotification>()
                        .Where(x => x.SenderId == sender)
                        .OrderByDescending(x => x.Date)
                        .Take(30)
                        .ToList();

                return result.Select(notes => new Notification
                {
                    Content = notes.Content,
                    Date = notes.Date,
                    ReceiversNames = GetReceivers(notes.Id)
                }).AsEnumerable().Cast<INotification>().ToList();
            }
        }

        public List<IMessage> GetMessages(int employeeId1, int employeeId2)
        {
            var employee1 = String.Format("RavenEmployees/{0}", employeeId1);
            var employee2 = String.Format("RavenEmployees/{0}", employeeId2);
            List<RavenMessage> result;
            var messages = new List<IMessage>();

            using (IDocumentSession session = _documentStore.OpenSession())
            {          
                session.Advanced.MaxNumberOfRequestsPerSession = 1000;

                result = session.Query<RavenMessage>()
                    .Customize(x => x.Include<RavenMessage>(o => o.ReceiverId))
                    .Customize(x => x.Include<RavenMessage>(o => o.SenderId))
                    .Where(
                        (x =>
                            (x.ReceiverId == employee1 && x.SenderId == employee2) ||
                            (x.ReceiverId == employee2 && x.SenderId == employee1)))
                    .OrderByDescending(x => x.Date).Take(Int32.MaxValue).ToList();

            }
            foreach (var message in result)
                {
                    using (var session = _documentStore.OpenSession())
                    {
                        messages.Add(new Message
                        {
                            Content = message.Content,
                            Date = message.Date,
                            ReceiverName = session.Load<RavenEmployee>(message.ReceiverId).Name,
                            SenderName = session.Load<RavenEmployee>(message.SenderId).Name
                        });            
                    }
                }

            return messages;

        }

        public void AddTimeofReading(int notificationId, int receiverId)
        {
            var notification = String.Format("RavenNotifications/{0}", notificationId);
            var receiver = String.Format("RavenEmployees/{0}", receiverId);

            using (var session = _documentStore.OpenSession())
            {
                var result =
                    session.Query<RavenReceiversOfNotification>()
                        .FirstOrDefault(x => (x.NotificationId == notification && x.ReceiverId == receiver));

                if (result != null && result.WhenRead == DateTime.MinValue)
                {
                    result.WhenRead = DateTime.Now;
                    session.SaveChanges();
                }
            }
        }

        public void AddEmployee(IEmployee employee)
        {
            using (var session = _documentStore.OpenSession())
            {
                var result =
                    session.Query<RavenEmployee>().FirstOrDefault(x => x.EmployeeId == employee.EmployeeId);

                if (result != null) return;

                var newEmployee = new RavenEmployee
                {
                    EmployeeId = employee.EmployeeId,
                    Name = employee.Name
                };

                _documentStore.Conventions.RegisterIdConvention<RavenEmployee>(
                    (dbname, commands, user) => "RavenEmployees/" + user.EmployeeId);

                session.Store(newEmployee);
                session.SaveChanges();
            }
        }

        private List<string> GetReceivers(string notesId)
        {
            var lista = new List<string>();

            using (var session = _documentStore.OpenSession())
            {
                session.Advanced.MaxNumberOfRequestsPerSession = 1000;

                var result = session.Query<RavenReceiversOfNotification>()
                    .Customize(x => x.Include<RavenReceiversOfNotification>(o => o.ReceiverId))
                    .Where(x => x.NotificationId == notesId)
                    .ToList();

                foreach (var r in result)
                {
                    var s = session.Load<RavenEmployee>(r.ReceiverId).Name;

                    if (r.WhenRead != DateTime.MinValue)
                    {
                        s += " (odczytane: " + GetDateTimeString(r.WhenRead) + ")";
                    }
                    else
                    {
                        s += " (nie odczytane)";
                    }
                    lista.Add(s);
                }
                return lista;
            }
        }

        private static string GetDateTimeString(DateTime date)
        {
            if (date.ToShortDateString() == DateTime.Now.ToShortDateString())
                return String.Format("dzisiaj, {0}", date.ToLongTimeString());
            return String.Format("{0}r., {1}", date.ToString("dd.MM.yyyy"), date.ToLongTimeString());
        }
    }
}