using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Notifications.Base;
using Notifications.BusiessLogic;

namespace $rootnamespace$.Hubs
{
    public class ChatHub : Hub
    {
        private static readonly List<Employee> ConnectedUsers = new List<Employee>();
        private readonly IChatApplication _application;

        public ChatHub()
        {
            _application = new ChatApplication(new Factory(new RavenRepository("http://localhost:8080", "chat")));
        }


        public void Connect(string userName, int userId)
        {
            string id = Context.ConnectionId;

            if (ConnectedUsers.Count(x => x.ConnectionId == id) == 0)
            {
                var employee = new Employee { ConnectionId = id, Name = userName, EmployeeId = userId };
                ConnectedUsers.Add(employee);

                _application.AddEmployee(employee);


                Clients.Caller.onConnected(id, userName, ConnectedUsers); // send list of active person to caller

                Clients.AllExcept(id).onNewUserConnected(id, userName); // send to all except caller client

                Clients.All.onlineUsers(ConnectedUsers.Count - 1); //send actual number of available users
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


    }
}