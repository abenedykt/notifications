using Notifications.DataAccessLayer.RavenClass;
using Raven.Client;
using Raven.Client.Document;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication2
{
    internal class Program
    {
        public static DocumentStore DocumentStore2 = new DocumentStore
        {
            Url = "http://localhost:8080",
            DefaultDatabase = "chat2"
        };

        public static DocumentStore DocumentStore1 = new DocumentStore
        {
            Url = "http://localhost:8080",
            DefaultDatabase = "chat"
        };

        private static void Main(string[] args)
        {
            DocumentStore1.Initialize();
            DocumentStore2.Initialize();

            var timeMessage2 = AddMessageToChat(DocumentStore2);
            var timeMessage1 = AddMessageToChat(DocumentStore1);


            var timeNote1 = AddNotificationToChat(DocumentStore1);
            var timeNote2 = AddNotificationToChat(DocumentStore2);

            Console.WriteLine("Dodawanie wiadomości do chat zapelniony: " + timeMessage1);
            Console.WriteLine("Dodawanie wiadomości do chat pusty: " + timeMessage2);
            Console.WriteLine("");
            Console.WriteLine("Dodawanie powiadomienień do chat zapelniony: " + timeNote1);
            Console.WriteLine("Dodawanie powiadomienień do chat pusty: " + timeNote2);
            Console.ReadLine();
        }


        public static long AddMessageToChat(IDocumentStore documentStore)
        {
            var watch = Stopwatch.StartNew();

            using (IDocumentSession session = documentStore.OpenSession())
            {
                for (int i = 1; i <= 100000; i++)
                {
                    var ravenMessage = new RavenMessage
                    {
                        Content = "wiadomosc nr" + i,
                        Date = new DateTime(2012, 11, 20, 12, 12, 12),
                        ReceiverId = String.Format("RavenEmployees/{0}", 100),
                        SenderId = String.Format("RavenEmployees/{0}", 101)
                    };
                    session.Store(ravenMessage);
                }
                session.SaveChanges();
            }
            watch.Stop();

            return watch.ElapsedMilliseconds;
        }

        public static long AddNotificationToChat(IDocumentStore documentStore)
        {
            var watch = Stopwatch.StartNew();

            for (int j = 1; j < 5000; j++)
            {
                using (IDocumentSession session = documentStore.OpenSession())
                {
                    var ravenNotification = new RavenNotification
                    {
                        Content = "powiadomienie nr" + j,
                        Date = new DateTime(2012, 11, 20, 12, 12, 12),
                        SenderId = String.Format("RavenEmployees/{0}", 1000)
                    };

                    session.Store(ravenNotification);

                    session.SaveChanges();

                    for (int i = 1; i <= 3; i++)
                    {
                        var receiverOfNotification = new RavenReceiversOfNotification
                        {
                            NotificationId = ravenNotification.Id,
                            ReceiverId = String.Format("RavenEmployees/{0}", i),
                            Date = ravenNotification.Date
                        };
                        session.Store(receiverOfNotification);
                    }

                    session.SaveChanges();
                }                
            }
            watch.Stop();

            return watch.ElapsedMilliseconds;
        }
    }
}