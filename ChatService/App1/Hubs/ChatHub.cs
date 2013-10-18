using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Client.Hubs;
using Notifications.BusiessLogic;

namespace App1.Hubs
{
    public class ChatHub : Hub
    {
        private static List<Employee> ConnectedUsers = new List<Employee>();

        public ChatHub()
        {
            AddClientMethod();
        }

        public void Connect(string userName, int userId)//polaczenie sie nowego klienta 
        {
            string id = Context.ConnectionId;
            Debug.WriteLine("Id uzytkownika dla {0} = {1}", userName, id);
            if (ConnectedUsers.Count(x => x.EmployeeId == userId) == 0)
            {
                var employee = new Employee { ConnectionId = id, Name = userName, EmployeeId = userId };
                ConnectedUsers.Add(employee);

                Clients.Caller.onConnected(userId, ConnectedUsers); // send list of active person to caller

                Clients.AllExcept(id).onNewUserConnected(userId, userName); // send to all except caller client

                Clients.All.onlineUsers(ConnectedUsers.Count - 1); //send actual number of available users

                GlobalVar.chat.Invoke("ConnectUser", userName, userId);
            }
            else
            {
                ConnectedUsers.FirstOrDefault(x => x.EmployeeId == userId).ConnectionId = Context.ConnectionId;
                Clients.Caller.onConnected(userId, ConnectedUsers); // send list of active person to caller
                Clients.Caller.onlineUsers(ConnectedUsers.Count - 1); //send actual number of available users
            }
        }

        public override Task OnDisconnected() //w przypadku rozlaczenia sie uzytkownika
        {
            var id = Context.ConnectionId;

            Thread.Sleep(1000);
            Employee item = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == id);
            if (item != null)
            {
                ConnectedUsers.Remove(item);

                Clients.All.onUserDisconnected(item.EmployeeId, item.Name);
                Clients.All.onlineUsers(ConnectedUsers.Count - 1); //send actual number of available users
                GlobalVar.chat.Invoke("OnUserDisconnected", item.EmployeeId);

            }
            return base.OnDisconnected();
        }

        public void SendDisconnectUser(int disconnectUserId)
        {
            var employee = ConnectedUsers.FirstOrDefault(x => x.EmployeeId == disconnectUserId);
            if (employee == null) return;
            ConnectedUsers.Remove(employee);
            Clients.All.onUserDisconnected(employee.EmployeeId, employee.Name); //odswiezanie listy aktywnych
            Clients.All.onlineUsers(ConnectedUsers.Count - 1);
        }

        public void SendConnectUser(int[] ConnectUsersIds)
        {
            var id = Context.ConnectionId;
            var employee = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == id);
            var result = ConnectedUsers.FindAll(user => ConnectUsersIds.Any(x => x == user.EmployeeId)).ToList();
            ConnectedUsers = result;
            Clients.All.onConnected(employee.EmployeeId, ConnectedUsers);
            Clients.All.onlineUsers(ConnectedUsers.Count - 1);
        }


        public void OnNewUserConnected(int userId, string userName)
        {
            if (ConnectedUsers.Count(x => x.EmployeeId == userId) == 0)
            {
                var employee = new Employee { Name = userName, EmployeeId = userId };
                ConnectedUsers.Add(employee);

                Clients.All.onNewUserConnected(userId, userName); // send to all except caller client

                Clients.All.onlineUsers(ConnectedUsers.Count - 1); //send actual number of available users
            }

        }

        private void AddClientMethod()
        {
            GlobalVar.chat.On<int, string>("OnNewUserConnected", OnNewUserConnected);
            GlobalVar.chat.On<int>("SendDisconnectUser", SendDisconnectUser);
            GlobalVar.chat.On<int>("SendConnectUser", SendDisconnectUser);
        }
    }
}