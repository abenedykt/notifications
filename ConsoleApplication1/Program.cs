using Notifications.DataAccessLayer.RavenClass;
using Raven.Client;
using Raven.Client.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.Client.Shard;

namespace ConsoleApplication1
{
    class Program
    {
        static DocumentStore documentStore = new DocumentStore
        {
            Url = "http://localhost:8080"
        };

        static void Main(string[] args)
        {

            documentStore.Initialize();

            //addMessages();
            //addEmployees();
            addNotifications();

            Console.WriteLine("koniec!");
            Console.ReadLine();
        }

        public static void addEmployees()
        {

        using(IDocumentSession _session = documentStore.OpenSession())
            {
            for (int i = 1001; i <= 5000; i++)
            {
                var newEmployee = new RavenEmployee
                {
                    EmployeeId = i,
                    Name = string.Format("Adam{0}", i)
                };
                _session.Store(newEmployee);
            }
            _session.SaveChanges();
            }
        }

        public static void addMessages()
        {
            
            Random r = new Random();            
            for (int j = 0; j < 1000; j++)
            {
                using (IDocumentSession _session = documentStore.OpenSession())
                {
                    var sender = _session.Query<RavenEmployee>().FirstOrDefault(x => x.EmployeeId == r.Next(1, 1000));
                    var recepient = _session.Query<RavenEmployee>().FirstOrDefault(x => x.EmployeeId == r.Next(1, 1000));
                    
                    for (int i = 1; i < 1000; i++)
                    {
                        var ravenMessage = new RavenMessage
                        {
                            MessageId = i,
                            Content = "wiadomosc nr" + i,
                            Date = new DateTime(2012, 11, 20),
                            Receiver = recepient,
                            Sender = sender
                        };
                        _session.Store(ravenMessage);
                    }

                    _session.SaveChanges();
                    Console.WriteLine("koniec " + j );
                }
            }

            using (IDocumentSession _session = documentStore.OpenSession())
            {
                var table = _session.Query<RavenMessage>().Take(10000).ToArray();
                foreach (var ravenMessage in table)
                {
                    Console.WriteLine(string.Format("{0}, nadawca: {1}, odbiorca: {2}, treść: {3}",
                        ravenMessage.MessageId, ravenMessage.Sender.Name, ravenMessage.Receiver.Name,
                        ravenMessage.Content));
                }
            }
        }

        public static void addNotifications()
        {

            Random r = new Random();
            
            for (int j=1001 ; j < 100000; j++)
            {
                using (IDocumentSession _session = documentStore.OpenSession())
                {
                    var sender = _session.Query<RavenEmployee>().FirstOrDefault(x => x.EmployeeId == r.Next(1, 1000));

                    var ravenNotification = new RavenNotification
                    {
                        NotificationId = j,
                        Content = "powiadomienie nr" + j,
                        Date = new DateTime(2012, 11, 20),
                        Sender = sender
                    };

                    _session.Store(ravenNotification);

                    _session.SaveChanges();

                    RavenEmployee recepient = null;

                    for (int i = 0; i < r.Next(1,5); i++)
                    {
                        recepient = _session.Query<RavenEmployee>().FirstOrDefault(x => x.EmployeeId == r.Next(1, 1000));
                        ravenNotification.Receivers.Add(recepient);

                        var receiverOfNotification = new RavenReceiversOfNotification
                        {
                            Notification = ravenNotification,
                            Receiver = recepient,
                            ReceiversOfNotificationId = j
                        };

                        _session.Store(receiverOfNotification);
                        _session.SaveChanges();
                    }

                }
            }

          
            
        }

    }
}
