using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Notifications.BusiessLogic;
using Notifications.Base;
using System.Web.Mvc;

namespace Notifications.Mvc.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IApplication _application;
        public ChatHub(IApplication application)
        {
            _application = application;
        }

        static List<Employee> ConnectedUsers = new List<Employee>();

        public void Connect(string userName, int userId)
        {
            var id = Context.ConnectionId;

            if (ConnectedUsers.Count(x => x.ConnectionId == id) == 0)
            {
                ConnectedUsers.Add(new Employee { ConnectionId = id, Name = userName, EmployeeId = userId });

                Clients.Caller.onConnected(id, userName, ConnectedUsers); // send list of active person to caller

                Clients.AllExcept(id).onNewUserConnected(id, userName); // send to all except caller client

                Clients.All.onlineUsers(ConnectedUsers.Count - 1); //send actual number of available users

                List<INotification> receiveNotes = _application.GetReceiveNotifications(userId); //get history of received messages
                foreach (var note in receiveNotes)
                    Clients.Caller.getReceivedNotifications(GetDateTimeString(note.Date), note.SenderName, note.Content);

                List<INotification> sendNotes = _application.GetSendNotifications(userId); //get history of send messages
                foreach(var note in sendNotes)             
                    Clients.Caller.getSendNotifications(GetDateTimeString(note.Date), GetReceiversString(note.ReceiversNames), note.Content);
                }                
            }
        

        string GetReceiversString(List<string> receiversList) //method for history of send messages
        {           
            var receivers = "";

            foreach(var receiver in receiversList)
            {
                if (receivers != "") receivers += ", " + receiver;
                else receivers += receiver;
            }

            return receivers;              
        }

        public override Task OnDisconnected()
        {
            var item = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            if (item != null)
            {
                ConnectedUsers.Remove(item);

                var id = Context.ConnectionId;
                Clients.All.onUserDisconnected(id, item.Name);
            }

            Clients.All.onlineUsers(ConnectedUsers.Count - 1); //send actual number of available users

            return base.OnDisconnected();
        }

        public async Task SendPrivateMessage(string toUserId, string message)
        {
            string fromUserId = Context.ConnectionId;

            var toUser = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == toUserId);
            var fromUser = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == fromUserId);

            if (toUser != null && fromUser != null)
            {
                await Clients.Client(toUserId).createNewWindow(fromUserId, fromUser.Name, message);
            } 
        }

        public async Task SendMessage(bool newWindow, string toUserId, string fromUserName, string message)
        {
            string fromUserId = Context.ConnectionId;

            var toUser = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == toUserId);
            var fromUser = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == fromUserId);
            DateTime date = DateTime.Now;
            if (newWindow) 
            {
                await GetMessages(toUserId);
            }
            await Clients.Caller.addMessage(toUserId, fromUserName, message, GetDateTimeString(date)); // send to caller user

            await Clients.Client(toUserId).addMessage(fromUserId, fromUserName, message, GetDateTimeString(date)); // send to 

            _application.SendMessage(message, fromUser.EmployeeId, toUser.EmployeeId, date);
        }


        public async Task Broadcasting(string[] users, string notification, string userName)
        {
            string fromUserId = Context.ConnectionId;
            var fromUser = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == fromUserId);

            DateTime date = DateTime.Now;

            var receivers = new List<int>();
            var receiversName = "";

            foreach (var item in users)
            {
                await this.Groups.Add(item, "grupa");

                var toUser = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == item);
                receivers.Add(toUser.EmployeeId);

                if (receiversName != "") receiversName += ", " + toUser.Name;
                else receiversName = toUser.Name;
            }

            await Clients.Group("grupa").sendNotificationBroadcast(notification, userName);

            _application.BrodcastNotification(notification, fromUser.EmployeeId, receivers, date);

            foreach (var item in users)
            {
                Clients.Client(item).getReceivedNotifications(GetDateTimeString(date), userName, notification);

                await this.Groups.Remove(item, "grupa");
            }

            Clients.Caller.getSendNotifications(GetDateTimeString(date), receiversName, notification);
               Clients.Caller.confirmation();
        }


        string GetDateTimeString(DateTime date)
        {
            if (date.ToShortDateString() == DateTime.Now.ToShortDateString())
                return  String.Format("Dzisiaj, {0}",  date.ToLongTimeString());
            else
                return String.Format("{0}r., {1}", date.ToString("dd.MM.yyyy"), date.ToLongTimeString());
          
        }

        public async Task GetMessages(string toUserId)
        {
            string fromUserId = Context.ConnectionId;

            var toUser = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == toUserId);
            var fromUser = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == fromUserId);

            var messages = _application.GetMessages(toUser.EmployeeId, fromUser.EmployeeId);
            foreach (var m in messages)
            {
                await Clients.Caller.addMessage(toUserId, m.SenderName, m.Content, GetDateTimeString(m.Date));
            }
        }

          }
}