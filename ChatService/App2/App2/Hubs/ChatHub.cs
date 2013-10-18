using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Client.Hubs;
using Notifications.BusiessLogic;

namespace App2.Hubs
{
    public class ChatHub : Hub
    {
        private static List<Employee> ConnectedUsers = new List<Employee>();

        public ChatHub()
        {
            AddClientMethod();
        }

        public async void Connect(string userName, int userId)//polaczenie sie nowego klienta 
        {
            string id = Context.ConnectionId;
            Debug.WriteLine("Id uzytkownika dla {0} = {1}", userName, id);
            if (ConnectedUsers.Count(x => x.EmployeeId == userId) == 0)
            {
                var employee = new Employee { ConnectionId = id, Name = userName, EmployeeId = userId };
                ConnectedUsers.Add(employee);

                await Clients.Caller.onConnected(userId, ConnectedUsers); // send list of active person to caller

                await Clients.AllExcept(id).onNewUserConnected(userId, userName); // send to all except caller client

                await Clients.All.onlineUsers(ConnectedUsers.Count - 1); //send actual number of available users

                GlobalVar.chat.Invoke("ConnectUser", userName, userId);
            }
            else
            {
                ConnectedUsers.FirstOrDefault(x => x.EmployeeId == userId).ConnectionId = Context.ConnectionId;
                await Clients.Caller.onConnected(userId, ConnectedUsers); // send list of active person to caller
                await Clients.Caller.onlineUsers(ConnectedUsers.Count - 1); //send actual number of available users
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


        public async Task PrivateMessage(int toUserId, string message)
        {
            var fromUser = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);

            await GlobalVar.chat.Invoke("PrivateMessage", fromUser.EmployeeId, toUserId, message);
        }

        public async Task SendMessage(bool newWindow, int fromUserId, string fromUserName, string message)
        {
            string toUserId = Context.ConnectionId;

            Employee toUser = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == toUserId);
            Employee fromUser = ConnectedUsers.FirstOrDefault(x => x.EmployeeId == fromUserId);

            DateTime date = DateTime.Now;

            await Clients.Caller.addMessage(fromUserId, fromUserName, message, GetDateTimeString(date));
            // send to caller user

            GlobalVar.chat.Invoke(toUserId, fromUserId, fromUserName, message, GetDateTimeString(date));

        }




        public async void SendDisconnectUser(int disconnectUserId)
        {
            var employee = ConnectedUsers.FirstOrDefault(x => x.EmployeeId == disconnectUserId);
            if (employee == null) return;
            ConnectedUsers.Remove(employee);
            await Clients.All.onUserDisconnected(employee.EmployeeId, employee.Name); //odswiezanie listy aktywnych
            await Clients.All.onlineUsers(ConnectedUsers.Count - 1);
        }

        public async void SendConnectUser(int[] ConnectUsersIds)
        {
            var id = Context.ConnectionId;
            var employee = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == id);
            var result = ConnectedUsers.FindAll(user => ConnectUsersIds.Any(x => x == user.EmployeeId)).ToList();
            ConnectedUsers = result;
            await Clients.All.onConnected(employee.EmployeeId, ConnectedUsers);
            await Clients.All.onlineUsers(ConnectedUsers.Count - 1);
        }

        public async void OnNewUserConnected(int userId, string userName)
        {
            if (ConnectedUsers.Count(x => x.EmployeeId == userId) == 0)
            {
                var employee = new Employee { Name = userName, EmployeeId = userId };
                ConnectedUsers.Add(employee);

                await Clients.All.onNewUserConnected(userId, userName); // send to all except caller client

                await Clients.All.onlineUsers(ConnectedUsers.Count - 1); //send actual number of available users
            }
        }

        private void AddClientMethod()
        {
            GlobalVar.chat.On<int, string>("OnNewUserConnected", OnNewUserConnected);
            GlobalVar.chat.On<int>("SendDisconnectUser", SendDisconnectUser);
            GlobalVar.chat.On<int>("SendConnectUser", SendDisconnectUser);
            GlobalVar.chat.On<int, int, string>("CreatePrivateWindow", CreatePrivateWindow);
            GlobalVar.chat.On<int, int, string, string, string>("AddMessage", AddMessage);
        }

        public async void AddMessage(int toUserId, int fromUserId, string fromUserName, string message, string date)
        {
            Employee fromUser = ConnectedUsers.FirstOrDefault(x => x.EmployeeId == fromUserId);

            await Clients.Client(fromUser.ConnectionId).addMessage(toUserId, fromUserName, message, date);
        }

        private async void CreatePrivateWindow(int fromUserId, int toUserId, string message)
        {
            Employee toUser = ConnectedUsers.FirstOrDefault(x => x.EmployeeId == toUserId);
            Employee fromUser = ConnectedUsers.FirstOrDefault(x => x.EmployeeId == fromUserId);

            if (toUser != null && fromUser != null)
            {
                await Clients.Client(toUser.ConnectionId).createNewWindow(fromUser.EmployeeId, fromUser.Name, message);
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