using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Notifications.Base;
using Notifications.BusiessLogic;
using Notifications.DataAccessLayer;
using System.Diagnostics;

namespace WebApp
{
    public class ChatHub : Hub
    {
        private static List<Employee> ConnectedUsers = new List<Employee>();
        private readonly IChatApplication _application;
        private bool _saving;


        public ChatHub()
        {
            _saving = false;

            if (_saving)
            {
                var mongoConnection = new MongoStringConnection
                {
                    DatabaseName = "chat",
                    DatabaseUrl = "mongodb://emp:12345@localhost/chat"
                };

                _application = new ChatApplication(new Factory(new MongoRepository(mongoConnection)));
            }
            
        }

        public void HelloWorld()
        {
            Clients.Caller.addText();
        }

        public void Connect(string userName, int userId)
        {
            string id = Context.ConnectionId;

            if (ConnectedUsers.Count(x => x.EmployeeId == userId) == 0)
            {
                var employee = new Employee { ConnectionId = id, Name = userName, EmployeeId = userId };
                ConnectedUsers.Add(employee);

                if(_saving) _application.AddEmployee(employee);

                Clients.Caller.onConnected(userId, userName, ConnectedUsers);
                Clients.AllExcept(id).onNewUserConnected(userId, userName);
                Clients.All.onlineUsers(ConnectedUsers.Count - 1);
            }
            else
            {
                ConnectedUsers.FirstOrDefault(x => x.EmployeeId == userId).ConnectionId = Context.ConnectionId;
                Clients.Caller.onConnected(userId, userName, ConnectedUsers);
                Clients.Caller.onlineUsers(ConnectedUsers.Count - 1); 
            }
        }
       
        public override Task OnDisconnected()
        {
            var id = Context.ConnectionId;

            Thread.Sleep(1000);
            Employee item = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == id);
            if (item != null)
            {
                ConnectedUsers.Remove(item);

                Clients.All.onUserDisconnected(item.EmployeeId, item.Name);
                Clients.All.onlineUsers(ConnectedUsers.Count - 1);
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

            if (newWindow && _saving) await GetHistory(toUserId);
            
            await Clients.Caller.addMessage(toUserId, fromUserName, message, date.ToShortTimeString());
            await Clients.Client(toUser.ConnectionId).addMessage(fromUser.EmployeeId, "Ja", message, date.ToShortTimeString());
        }

        public async Task GetHistory(int toUserId)
        {
            if (_saving)
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
        }
  
        private string GetDateTimeString(DateTime date)
        {
            if (date.ToShortDateString() == DateTime.Now.ToShortDateString())
                return String.Format("{0}", date.ToShortTimeString());
            return String.Format("{0}r.,{1}", date.ToString("dd.MM.yyyy"), date.ToShortTimeString());
        }
    }    
}