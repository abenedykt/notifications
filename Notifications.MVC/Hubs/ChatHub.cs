using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Notifications.Base;
using Notifications.BusiessLogic;

namespace Notifications.Mvc.Hubs
{
    public class ChatHub : Hub
    {
        private static readonly List<Employee> ConnectedUsers = new List<Employee>();
        private readonly IApplication _application;

        public ChatHub(IApplication application)
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

                Clients.All.onlineUsers(ConnectedUsers.Count - 1); //send actual number of available users

                //List<INotification> receiveNotes = _application.GetReceiveNotifications(userId);
                    //get history of received messages
                //foreach (INotification note in receiveNotes)
               //     Clients.Caller.getReceivedNotifications(GetDateTimeString(note.Date), note.SenderName, note.Content);

               // List<INotification> sendNotes = _application.GetSendNotifications(userId);
                    //get history of send messages
                //foreach (INotification note in sendNotes)
                //    Clients.Caller.getSendNotifications(GetDateTimeString(note.Date),
                  //      GetReceiversNamesString(note.ReceiversNames), note.Content);
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

            Clients.All.onlineUsers(ConnectedUsers.Count - 1); //send actual number of available users

            return base.OnDisconnected();
        }

        public async Task PrivateMessage(string toUserId, string message)
        {
            string fromUserId = Context.ConnectionId;

            Employee toUser = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == toUserId);
            Employee fromUser = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == fromUserId);

            if (toUser != null && fromUser != null)
            {
                await Clients.Client(toUserId).createNewWindow(fromUserId, fromUser.Name, message);
            }
        }

        public async Task SendMessage(bool newWindow, string toUserId, string fromUserName, string message)
        {
            string fromUserId = Context.ConnectionId;

            Employee toUser = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == toUserId);
            Employee fromUser = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == fromUserId);

            DateTime date = DateTime.Now;

            if (newWindow)
            {
                await GetHistory(toUserId);
            }

            await Clients.Caller.addMessage(toUserId, fromUserName, message, GetDateTimeString(date));
                // send to caller user

            await Clients.Client(toUserId).addMessage(fromUserId, fromUserName, message, GetDateTimeString(date));
                // send to 

            _application.SendMessage(message, toUser.EmployeeId, fromUser.EmployeeId, date);
        }

        public async Task Broadcasting(string[] users, string notification, string userName)
        {
            Employee fromUser = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);

            DateTime date = DateTime.Now;

            var receiversIds = new List<int>();
            var receiversNames = new List<string>();

            foreach (string item in users)
            {
                await Groups.Add(item, "broadcasting");

                Employee toUser = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == item);
                receiversIds.Add(toUser.EmployeeId);
                receiversNames.Add(toUser.Name);
            }

            await Clients.Group("broadcasting").sendNotificationBroadcast(notification, userName);

            _application.BrodcastNotification(notification, fromUser.EmployeeId, receiversIds, date);

            foreach (string item in users)
            {
                Clients.Client(item).getReceivedNotifications(GetDateTimeString(date), userName, notification);

                await Groups.Remove(item, "broadcasting");
            }

            Clients.Caller.getSendNotifications(GetDateTimeString(date), GetReceiversNamesString(receiversNames),
                notification);
            Clients.Caller.notificationConfirm();
        }

        public async Task GetHistory(string toUserId)
        {
            string fromUserId = Context.ConnectionId;

            Employee toUser = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == toUserId);
            Employee fromUser = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == fromUserId);

            List<IMessage> messages = _application.GetMessages(toUser.EmployeeId, fromUser.EmployeeId);
            foreach (IMessage m in messages)
            {
                await Clients.Caller.addMessage(toUserId, m.SenderName, m.Content, GetDateTimeString(m.Date));
            }
        }

        private string GetDateTimeString(DateTime date)
        {
            if (date.ToShortDateString() == DateTime.Now.ToShortDateString())
                return String.Format("Dzisiaj, {0}", date.ToLongTimeString());
            return String.Format("{0}r., {1}", date.ToString("dd.MM.yyyy"), date.ToLongTimeString());
        }

        private string GetReceiversNamesString(List<string> receiversList) //method for history of send messages
        {
            string receivers = "";

            foreach (string receiver in receiversList)
            {
                if (receivers != "") receivers += ", " + receiver;
                else receivers += receiver;
            }

            return receivers;
        }
    }
}