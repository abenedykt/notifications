﻿using System;
using Notifications.DataAccessLayer.RavenClass;
using Raven.Client;
using Raven.Client.Document;

namespace ConsoleApplication1
{
    internal class Program
    {
        public static DocumentStore DocumentStore = new DocumentStore
        {
            Url = "http://localhost:8080",
            DefaultDatabase = "chat"
        };

        private static void Main()
        {
            DocumentStore.Initialize();

            //AddEmployees();

            
               // AddMessages();
                AddNotifications();
            
           
            Console.WriteLine("koniec!");
            Console.ReadLine();
        }

        public static void AddEmployees()
        {
           
            using (IDocumentSession session = DocumentStore.OpenSession())
            {
                for (int i = 800001; i <= 1000000; i++)
                {
                    var newEmployee = new RavenEmployee
                    {
                        EmployeeId = i,
                        Name = string.Format("Adam{0}", i)
                    };
                    DocumentStore.Conventions.RegisterIdConvention<RavenEmployee>(
                        (dbname, commands, user) => "RavenEmployees/" + i);

                    session.Store(newEmployee);

                    if (i % 100000 == 0) session.SaveChanges();
                }
                session.SaveChanges();
            }
        }

        public static void AddMessages()
        {
            var r = new Random();

            for (int j = 1; j < 10000; j++)
            {
                using (IDocumentSession session = DocumentStore.OpenSession())
                {
                    for (int i = 1000*j; i < 1000*(j + 1); i++)
                    {
                        var ravenMessage = new RavenMessage
                        {
                            Content = "wiadomosc nr" + i,
                            Date = new DateTime(2012, 11, 20),
                            ReceiverId = String.Format("RavenEmployees/{0}", r.Next(1, 300000)),
                            SenderId = String.Format("RavenEmployees/{0}", r.Next(1, 300000))
                        };
                        session.Store(ravenMessage);
                    }
                    session.SaveChanges();
                }
            }
        }

        public static void AddNotifications()
        {
            var r = new Random();

            for (int j = 300000; j < 1000000; j++)
            {
                using (IDocumentSession session = DocumentStore.OpenSession())
                {
                    var ravenNotification = new RavenNotification
                    {
                        Content = "powiadomienie nr" + j,
                        Date = DateTime.Now,
                        SenderId = String.Format("RavenEmployees/{0}", r.Next(1, 300000))
                    };

                    session.Store(ravenNotification);

                    session.SaveChanges();

                    for (int i = 0; i < r.Next(1, 5); i++)
                    {
                        var receiverOfNotification = new RavenReceiversOfNotification
                        {
                            NotificationId = ravenNotification.Id,
                            ReceiverId = String.Format("RavenEmployees/{0}", r.Next(1, 300000)),
                            Date = ravenNotification.Date
                        };
                        session.Store(receiverOfNotification);
                        session.SaveChanges();
                    }
                }
            }
        }
    }
}