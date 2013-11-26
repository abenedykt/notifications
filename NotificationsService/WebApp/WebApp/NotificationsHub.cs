using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Notifications.BusiessLogic;
using Notifications.Base;
using Notifications.DataAccessLayer;
using System.Threading.Tasks;
using System.Threading;

namespace WebApp
{
    public class NotificationsHub : Hub
    {
        private static readonly List<Employee> ConnectedUsers = new List<Employee>();
        private readonly IChatApplication _application;

        public NotificationsHub()
        {
            var mongoConnection = new MongoStringConnection
            {
                DatabaseName = "chat",
                DatabaseUrl = "mongodb://emp:12345@localhost/chat"
            };
            _application = new ChatApplication(new Factory(new MongoRepository(mongoConnection)));
        }

        public void Success()
        {
            Clients.Caller.addText();
        }

        public void Connect(string userName, string userId)
        {
            var id = Context.ConnectionId;

            if (ConnectedUsers.Count(x => x.EmployeeId == userId) == 0)
            { 
                var employee = new Employee { ConnectionId = id, Name = userName, EmployeeId = userId };
                ConnectedUsers.Add(employee);

                _application.AddEmployee(employee);

                Clients.All.onConnected(ConnectedUsers).Wait();
            }
            else
            {
                var user = ConnectedUsers.FirstOrDefault(x => x.EmployeeId == userId);
                if (user != null)
                    user.ConnectionId = Context.ConnectionId;
                Clients.Caller.onConnected(ConnectedUsers).Wait();
            }
        }

        public void GetSendNotifications(string userId)
        {
            List<INotification> sendNotes = _application.GetSendNotifications(userId);
            foreach (INotification note in sendNotes)
                Clients.Caller.getSendNotifications(GetDateTimeString(note.Date),
                    GetReceiversNamesString(note.ReceiversNames), note.Content);
        }

        public void GetReceivedNotifications(string userId)
        {
            List<INotification> receiveNotes = _application.GetReceiveNotifications(userId);
            foreach (INotification note in receiveNotes)
                Clients.Caller.getReceivedNotifications(GetDateTimeString(note.Date), note.SenderName, note.Content);
        }

        public override Task OnDisconnected()
        {
            var id = Context.ConnectionId;
            Thread.Sleep(1000);
            var item = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == id);
            if (item != null)
            {
                ConnectedUsers.Remove(item);
                Clients.All.onUserDisconnected(item.EmployeeId, ConnectedUsers).Wait();
            }
            return base.OnDisconnected();
        }

        public async Task Broadcasting(string[] users, string notification, string userName)
        {
            Employee fromUser = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);

            var date = DateTime.Now;

            var receiversIds = new List<string>();
            var receiversNames = new List<string>();

            foreach (string item in users)
            {
                await Groups.Add(item, "broadcasting");

                Employee toUser = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == item);
                receiversIds.Add(toUser.EmployeeId);
                receiversNames.Add(toUser.Name);
            }

            var notificationId = _application.BrodcastNotification(notification, fromUser.EmployeeId, receiversIds, date);

            await Clients.Group("broadcasting").sendNotificationBroadcast(notificationId, notification, userName, fromUser.EmployeeId); 

            foreach (string item in users)
            {
                Clients.Client(item).addReceivedNotification(GetDateTimeString(date), userName, notification);

                await Groups.Remove(item, "broadcasting");
            }

            Clients.Caller.addSendNotification(GetDateTimeString(date), GetReceiversNamesString(receiversNames),
                notification);
            Clients.Caller.notificationConfirm();
        }
    
        public void AddTimeofReading(string notificationId, string senderId)
        {
            var userId = Context.ConnectionId;
            var user = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == userId);

            var sender = ConnectedUsers.FirstOrDefault(x => x.EmployeeId == senderId);

            _application.AddTimeofReading(notificationId, user.EmployeeId);

            List<INotification> sendNotes = _application.GetSendNotifications(senderId);

            Clients.Client(sender.ConnectionId).clearHistoryOfSendNotifications();
            // get history of send messages
            foreach (var note in sendNotes)
                Clients.Client(sender.ConnectionId).getSendNotifications(GetDateTimeString(note.Date),
                    GetReceiversNamesString(note.ReceiversNames), note.Content);
        }

        private string GetDateTimeString(DateTime date)
        {
            if (date.ToShortDateString() == DateTime.Now.ToShortDateString())
                return String.Format("Dzisiaj, {0}", date.ToLongTimeString());
            return String.Format("{0}r., {1}", date.ToString("dd.MM.yyyy"), date.ToLongTimeString());
        }

        private string GetReceiversNamesString(IEnumerable<string> receiversList)
        {
            string receivers = "";

            foreach (string receiver in receiversList)
            {
                if (receivers != "") receivers += ", <br/> " + receiver;
                else receivers += "<br/>" + receiver;
            }
            return receivers;
        }
    }
}