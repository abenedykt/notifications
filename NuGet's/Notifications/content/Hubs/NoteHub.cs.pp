using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Notifications.Base;
using Notifications.BusiessLogic;

namespace Notifications.Hubs
{
    public class NoteHub : Hub
    {
        private static readonly List<Employee> ConnectedUsers = new List<Employee>();
        private readonly IChatApplication _application;

        public NoteHub(IChatApplication application)
        {
            _application = application;
        }

        public void Connect(string userName, int userId)
        {
            string id = Context.ConnectionId;

            if (ConnectedUsers.Count(x => x.ConnectionId == id) == 0)
            {
                var employee = new Employee {ConnectionId = id, Name = userName, EmployeeId = userId};
                ConnectedUsers.Add(employee);

                _application.AddEmployee(employee);


                Clients.Caller.onConnected(id, userName, ConnectedUsers); // send list of active person to caller

                Clients.AllExcept(id).onNewUserConnected(id, userName); // send to all except caller client


                List<INotification> sendNotes = _application.GetSendNotifications(userId);
                // get history of send messages
                foreach (INotification note in sendNotes)
                    Clients.Caller.getSendNotifications(GetDateTimeString(note.Date),
                        GetReceiversNamesString(note.ReceiversNames), note.Content);

                List<INotification> receiveNotes = _application.GetReceiveNotifications(userId);
                // get history of received messages
                foreach (INotification note in receiveNotes)
                    Clients.Caller.getReceivedNotifications(GetDateTimeString(note.Date), note.SenderName, note.Content);
            }
        }

        public override Task OnDisconnected()
        {
            Employee item = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            if (item != null)
            {
                ConnectedUsers.Remove(item);

                string id = Context.ConnectionId;
                Clients.All.onUserDisconnected(id, item.Name);
            }

            return base.OnDisconnected();
        }

        public async Task Broadcasting(string[] users, string notification, string userName)
        {
            Employee fromUser = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);

            var date = DateTime.Now;

            var receiversIds = new List<int>();
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

       
        public void AddTimeofReading(int notificationId, int senderId)
        {
            var UserId = Context.ConnectionId;
            var User = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == UserId);

            Employee sender = ConnectedUsers.FirstOrDefault(x => x.EmployeeId == senderId);


            _application.AddTimeofReading(notificationId, User.EmployeeId);

            List<INotification> sendNotes = _application.GetSendNotifications(senderId);

            Clients.Client(sender.ConnectionId).clearHistoryOfSendNotifications();
            // get history of send messages
            foreach (INotification note in sendNotes)
                Clients.Client(sender.ConnectionId).getSendNotifications(GetDateTimeString(note.Date),
                    GetReceiversNamesString(note.ReceiversNames), note.Content);
        }

        private string GetDateTimeString(DateTime date)
        {
            if (date.ToShortDateString() == DateTime.Now.ToShortDateString())
                return String.Format("Dzisiaj, {0}", date.ToLongTimeString());
            return String.Format("{0}r., {1}", date.ToString("dd.MM.yyyy"), date.ToLongTimeString());
        }

        private string GetReceiversNamesString(IEnumerable<string> receiversList) //method for history of send messages
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