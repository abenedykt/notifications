using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Services;

using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Client.Hubs;
using Notifications.Base;
using Notifications.BusiessLogic;
using Notifications.DataAccessLayer;
using App2.ChatServiceReference;

namespace App2.Hubs
{
    public class ChatHub : Hub
    {
        private static readonly List<Employee> ConnectedUsers = new List<Employee>();
        private IHubProxy chat;
        private HubConnection connection;
        public ChatHub()
        {
            connection = new HubConnection("http://localhost:61122/signalr", useDefaultUrl: false);
            chat = connection.CreateHubProxy("ChatServiceHub");
            connection.Start().Wait();
        }

        public void Connect(string userName, int userId)//polaczenie sie nowego klienta 
        {
            string id = Context.ConnectionId;

            if (ConnectedUsers.Count(x => x.EmployeeId == userId) == 0)
            {
                var employee = new Employee { ConnectionId = id, Name = userName, EmployeeId = userId };
                ConnectedUsers.Add(employee);

                Clients.Caller.onConnected(userId, userName, ConnectedUsers); // send list of active person to caller

                Clients.AllExcept(id).onNewUserConnected(userId, userName); // send to all except caller client

                Clients.All.onlineUsers(ConnectedUsers.Count - 1); //send actual number of available users

                chat.Invoke("ConnectUser", userName, userId);
            }
            else
            {
                ConnectedUsers.FirstOrDefault(x => x.EmployeeId == userId).ConnectionId = Context.ConnectionId;
                Clients.Caller.onConnected(userId, userName, ConnectedUsers); // send list of active person to caller
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
                chat.Invoke("OnUserDisconnected", item.EmployeeId);

            }
            return base.OnDisconnected();
        }

        public void SendDisconnectUser(int disconnectUserId)
        {
            var employee = ConnectedUsers.FirstOrDefault(x => x.EmployeeId == disconnectUserId);
            ConnectedUsers.Remove(employee);
            Clients.All.onUserDisconnected(employee.EmployeeId, employee.Name);       //odswiezanie listy aktywnych
            Clients.All.onlineUsers(ConnectedUsers.Count - 1);

        }

    }
}