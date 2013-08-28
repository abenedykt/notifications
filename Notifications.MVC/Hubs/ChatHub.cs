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

                // send list of active person to caller
                Clients.Caller.onConnected(id, userName, ConnectedUsers);

                // send to all except caller client
                Clients.AllExcept(id).onNewUserConnected(id, userName);

                //send actual number of available users
                Clients.All.onlineUsers(ConnectedUsers.Count - 1);


                List<INotification> receiveNotes = _application.GetReceiveNotifications(userId);
                foreach (var note in receiveNotes)
                    Clients.Caller.getReceivedNotifications(note.Date.ToLongTimeString() + ", " + note.Date.ToLongDateString(), note.SenderName, note.Content);

                List<INotification> sendNotes = _application.GetSendNotifications(userId);
                foreach(var note in sendNotes)
                
                    Clients.Caller.getSendNotifications(note.Date.ToLongTimeString() + ", " + note.Date.ToLongDateString(), GetReceiversString(note.ReceiversNames), note.Content);
                }
                    
            }
        

        string GetReceiversString(List<string> receiversList)
        {
            
                var receivers = "";

                if (receiversList.Count > 0)
                    receivers = receiversList[0];

                for (int j = 1; j < receiversList.Count; j++)
                    receivers += ", " + receiversList[j];

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

            //send actual number of available users
            Clients.All.onlineUsers(ConnectedUsers.Count - 1);

            return base.OnDisconnected();
        }

        public void SendPrivateMessage(string toUserId, string message)
        {
            string fromUserId = Context.ConnectionId;

            var toUser = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == toUserId);
            var fromUser = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == fromUserId);

            DateTime date = DateTime.Now;

            if (toUser != null && fromUser != null)
            {
                // send to 
                Clients.Client(toUserId).sendPrivateMessage(fromUserId, fromUser.Name, message, date.ToLongTimeString());

                // send to caller user
                Clients.Caller.sendPrivateMessage(toUserId, fromUser.Name, message, date.ToLongTimeString());

                _application.SendMessage(message, fromUser.EmployeeId, toUser.EmployeeId, date);
            }
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
                Clients.Client(item).getReceivedNotifications(date.ToLongTimeString() + ", " + date.ToLongDateString(), userName, notification);

                await this.Groups.Remove(item, "grupa");
            }

            Clients.Caller.getSendNotifications(date.ToLongTimeString() + ", " + date.ToLongDateString(), receiversName, notification);
               Clients.Caller.confirmation();
        }

        public async Task GetMessages(string toUserId)
        {
            string fromUserId = Context.ConnectionId;

            var toUser = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == toUserId);
            var fromUser = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == fromUserId);

            var messages = _application.GetMessages(toUser.EmployeeId, fromUser.EmployeeId);
            foreach (var m in messages)
            { var date="";
            if (m.Date.ToShortDateString() == DateTime.Now.ToShortDateString())
                    date = "Dziś " + m.Date.ToLongTimeString();
                else
                    date = m.Date.ToString();

                await Clients.Caller.sendPrivateMessage(toUserId, m.SenderName, m.Content, date);
                
            }
        }

        //public void AddTimeOfReading() { }
    }
}