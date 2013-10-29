using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Notifications.Base;
using Notifications.BusiessLogic;
using Notifications.DataAccessLayer;

namespace $rootnamespace$.Hubs
{
    public class ChatHub : Hub
    {
        private static readonly List<Employee> ConnectedUsers = new List<Employee>();
        private readonly IChatApplication _application;

        public ChatHub()
        {
            var mongoConnection = new MongoStringConnection
            {
                DatabaseName = "chat",
                DatabaseUrl = "mongodb://emp:12345@localhost/chat"
            };

            _application = new ChatApplication(new Factory(new MongoRepository(mongoConnection)));
        }


        public void Connect(string userName, int userId)
        {
            string id = Context.ConnectionId;

            if (ConnectedUsers.Count(x => x.EmployeeId == userId) == 0)
            {
                var employee = new Employee { ConnectionId = id, Name = userName, EmployeeId = userId };
                ConnectedUsers.Add(employee);

                _application.AddEmployee(employee);

                Clients.Caller.onConnected(userId, userName, ConnectedUsers); // send list of active person to caller

                Clients.AllExcept(id).onNewUserConnected(userId, userName); // send to all except caller client

                Clients.All.onlineUsers(ConnectedUsers.Count - 1); //send actual number of available users
            }
            else
            {
                ConnectedUsers.FirstOrDefault(x => x.EmployeeId == userId).ConnectionId = Context.ConnectionId;
                Clients.Caller.onConnected(userId, userName, ConnectedUsers); // send list of active person to caller
                Clients.Caller.onlineUsers(ConnectedUsers.Count - 1); //send actual number of available users
            }


        }

        public override Task OnDisconnected()
        {
            var id = Context.ConnectionId;

            Thread.Sleep(2000);
            Employee item = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == id);
            if (item != null)
            {
                ConnectedUsers.Remove(item);

                Clients.All.onUserDisconnected(item.EmployeeId, item.Name);
                Clients.All.onlineUsers(ConnectedUsers.Count - 1); //send actual number of available users
            }
            return base.OnDisconnected();
        }

        public async Task PrivateMessage(int toUserId, string message)
        {
            string fromUserId = Context.ConnectionId;

            Employee toUser = ConnectedUsers.FirstOrDefault(x => x.EmployeeId == toUserId);
            Employee fromUser = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == fromUserId);

            if (toUser != null && fromUser != null)
            {
                await Clients.Client(toUser.ConnectionId).createNewWindow(fromUser.EmployeeId, fromUser.Name, message);
            }
        }

        public async Task SendMessage(bool newWindow, int toUserId, string fromUserName, string message)
        {
            string fromUserId = Context.ConnectionId;

            Employee toUser = ConnectedUsers.FirstOrDefault(x => x.EmployeeId == toUserId);
            Employee fromUser = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == fromUserId);

            DateTime date = DateTime.Now;

            if (newWindow)
            {
                await GetHistory(toUserId);
            }

            await Clients.Caller.addMessage(toUserId, fromUserName, message, GetDateTimeString(date));
            // send to caller user

            await Clients.Client(toUser.ConnectionId).addMessage(fromUser.EmployeeId, fromUserName, message, GetDateTimeString(date));
            // send to 

            _application.SendMessage(message, toUser.EmployeeId, fromUser.EmployeeId, date);
        }

        public async Task GetHistory(int toUserId)
        {
            string fromUserId = Context.ConnectionId;

            Employee toUser = ConnectedUsers.FirstOrDefault(x => x.EmployeeId == toUserId);
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