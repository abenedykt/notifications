using System;
using System.Collections.Generic;
using System.Linq;
using Notifications.Base;
using Notifications.BusiessLogic;
using Notifications.DataAccessLayer.MongoClass;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;

namespace Notifications.DataAccessLayer
{
    public class MongoRepository :IDataRepository
    {
        private readonly MongoDatabase _mongoDatabase;


        public MongoRepository(MongoStringConnection mongoConnection)
        {
            string connectionString = mongoConnection.DatabaseUrl;
            MongoClient mongoClient = new MongoClient(connectionString);
            MongoServer mongoServer = mongoClient.GetServer();
            _mongoDatabase = mongoServer.GetDatabase(mongoConnection.DatabaseName);
        }
      
        public string AddNotification(INotification notification)
        {
            var notifications = _mongoDatabase.GetCollection<MongoNotification>("Notifications");

            var mongoNotification = new MongoNotification
            {
               
                SenderId = notification.SenderId,
                Date = notification.Date,
                Content = notification.Content
            };
  
            notifications.Insert(mongoNotification);

            foreach (var receiver in notification.ReceiversIds)
            {
                var receiverOfNotification = new MongoReceiversOfNotification()
                {
                    ReceiverId = receiver,
                    NotificationId = mongoNotification.Id.ToString(),
                    Date = mongoNotification.Date
                };
                var mongoReceiversOfNotifications = _mongoDatabase.GetCollection<MongoReceiversOfNotification>("ReceiversOfNotifications");
                mongoReceiversOfNotifications.Insert(receiverOfNotification);
            }         
            return mongoNotification.Id.ToString();
        }

        public void AddMessage(IMessage message)
        {
            var mongoMessage = new MongoMessage
            {
                Content = message.Content,
                Date = message.Date,
                ReceiverId = message.ReceiverId,
                SenderId = message.SenderId
            };

            var messages = _mongoDatabase.GetCollection<MongoMessage>("Messages");
            messages.Insert(mongoMessage);


        }

        public List<INotification> GetReceiveNotifications(string receiverId)
        {
            var receiversOfNotifications = _mongoDatabase.GetCollection<MongoReceiversOfNotification>("ReceiversOfNotifications");
            var employees = _mongoDatabase.GetCollection<MongoEmployee>("Employees");
            var notifications = _mongoDatabase.GetCollection<MongoNotification>("Notifications");

            var result = receiversOfNotifications.AsQueryable().Where(x => x.ReceiverId == receiverId).OrderByDescending(x => x.Date)
                    .Take(30)
                    .ToList(); 


            var listOfNotification = new List<INotification>();

            foreach (var receiver in result)
            {
                 
                var employeeName = employees.AsQueryable().FirstOrDefault(x => x.EmployeeId == receiver.ReceiverId).Name;

                var n =ObjectId.Parse(receiver.NotificationId);

                var notification = notifications.AsQueryable().FirstOrDefault(x => x.Id == n);

                var note = new Notification()
                {
                    Content = notification.Content,
                    Date = notification.Date,
                    SenderName = employeeName
                };

                listOfNotification.Add(note);
            }

            return listOfNotification;
        }

        public List<INotification> GetSendNotifications(string senderId)
        {
            var notifications = _mongoDatabase.GetCollection<MongoNotification>("Notifications");

            var result =
                notifications.AsQueryable()
                    .Where(x => x.SenderId == senderId)
                    .OrderByDescending(x => x.Date)
                    .Take(30)
                    .ToList();

            return result.Select(notes => new Notification
            {
                Content = notes.Content,
                Date = notes.Date,
                ReceiversNames = GetReceivers(notes.Id.ToString())
            }).AsEnumerable().Cast<INotification>().ToList();

        }



        public List<IMessage> GetMessages(string employeeId1, string employeeId2)
        {
            var messages = _mongoDatabase.GetCollection<MongoMessage>("Messages");
            var employees = _mongoDatabase.GetCollection<MongoEmployee>("Employees");

            var listOfMessages = new List<IMessage>();
            
            var result = messages.AsQueryable().Where(
                        (x =>
                            (x.ReceiverId == employeeId1 && x.SenderId == employeeId2) ||
                            (x.ReceiverId == employeeId2 && x.SenderId == employeeId1)))
                    .OrderByDescending(x => x.Date).Take(10).ToList();


            foreach (var message in result.OrderBy(x=>x.Date))
            {
                var senderName =
                    employees.AsQueryable().FirstOrDefault(x => x.EmployeeId == message.SenderId).Name;
                var receiverName =
                    employees.AsQueryable().FirstOrDefault(x => x.EmployeeId == message.ReceiverId).Name;


                listOfMessages.Add(new Message
                {
                    Content = message.Content,
                    Date = message.Date,
                    ReceiverName = receiverName,
                    SenderName = senderName
                });
            }

            return listOfMessages;

        }


        private List<string> GetReceivers(string notesId)
        {
            var lista = new List<string>();

            var receiversOfNotifications = _mongoDatabase.GetCollection<MongoReceiversOfNotification>("ReceiversOfNotifications");
            var employees = _mongoDatabase.GetCollection<MongoEmployee>("Employees");

            var result = receiversOfNotifications.AsQueryable().Where(x => x.NotificationId == notesId).ToList();

            foreach (var receiver in result)
            {
                var name = employees.AsQueryable().FirstOrDefault(x => x.EmployeeId == receiver.ReceiverId).Name;

                if (receiver.WhenRead != DateTime.MinValue)
                {
                    name += " (odczytano: " + GetDateTimeString(receiver.WhenRead) + ")";
                }
                else
                {
                    name += " (nie odczytano)";
                }
                lista.Add(name);
            }
            return lista;
        }

        public void AddTimeofReading(string notificationId, string receiverId)
        {
            var receiversOfNotifications = _mongoDatabase.GetCollection<MongoReceiversOfNotification>("ReceiversOfNotifications");

            var result =
                receiversOfNotifications.AsQueryable()
                    .FirstOrDefault(x => x.NotificationId == notificationId && x.ReceiverId == receiverId);

            if (result != null && result.WhenRead == DateTime.MinValue)
            {
                var query = Query<MongoReceiversOfNotification>.EQ(e => e.Id, result.Id);

                var update = Update<MongoReceiversOfNotification>.Set(e => e.WhenRead, DateTime.Now);
                receiversOfNotifications.Update(query, update);
            }

        }

        private static string GetDateTimeString(DateTime date)
        {
            if (date.ToShortDateString() == DateTime.Now.ToShortDateString())
                return String.Format("dzisiaj, {0}", date.ToLongTimeString());
            return String.Format("{0}r., {1}", date.ToString("dd.MM.yyyy"), date.ToLongTimeString());
        }

        public void AddEmployee(IEmployee employee)
        {
            var employees = _mongoDatabase.GetCollection<MongoEmployee>("Employees");

            var result = employees.AsQueryable().FirstOrDefault(x => x.EmployeeId == employee.EmployeeId);

            if (result != null) return;

            var newEmployee = new MongoEmployee
                {
                    EmployeeId = employee.EmployeeId,
                    Name = employee.Name
                };

            employees.Insert(newEmployee);

        }
    }
}
