using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Client.Hubs;
using Notifications.Base;
using Notifications.BusiessLogic;

namespace App2.Hubs
{
    public class ChatHub : Hub
    {
        private static List<Employee> _connectedUsers = new List<Employee>();

        public ChatHub()
        {
            GlobalVar.chat.On<int, string>("OnNewUserConnected", OnNewUserConnected);
            GlobalVar.chat.On<int>("SendDisconnectUser", SendDisconnectUser);
            GlobalVar.chat.On<int>("SendConnectUsers", SendDisconnectUser);
            GlobalVar.chat.On<int, int, string, string>("CreatePrivateWindow", (fromUserId, toUserId, fromUserName, message) => CreatePrivateWindow(fromUserId, toUserId, fromUserName, message));
            //GlobalVar.chat.On<List<Message>, int, int, string, string>("AddMessages", (messages, fromUserId, toUserId, message, date) => AddMessages(messages, fromUserId, toUserId, message, date)); 
        }

        public async Task Connect(string userName, int userId)//polaczenie sie nowego klienta 
        {
            var id = Context.ConnectionId;
            Debug.WriteLine("Id uzytkownika dla {0} = {1}", userName, id);
            if (_connectedUsers.Count(x => x.EmployeeId == userId) == 0)
            {
                var employee = new Employee { ConnectionId = id, Name = userName, EmployeeId = userId };
                _connectedUsers.Add(employee);

                Clients.Caller.onConnected(userId, _connectedUsers); // send list of active person to caller

                Clients.AllExcept(id).onNewUserConnected(userId, userName); // send to all except caller client

                Clients.All.onlineUsers(_connectedUsers.Count - 1); //send actual number of available users

               await GlobalVar.chat.Invoke("ConnectUser", userName, userId);
            }
            else
            {
                _connectedUsers.FirstOrDefault(x => x.EmployeeId == userId).ConnectionId = Context.ConnectionId;
                Clients.Caller.onConnected(userId, _connectedUsers); // send list of active person to caller
                Clients.Caller.onlineUsers(_connectedUsers.Count - 1); //send actual number of available users
            }
        }

        public override Task OnDisconnected() //w przypadku rozlaczenia sie uzytkownika
        {
            var id = Context.ConnectionId;

            Thread.Sleep(1000);
            Employee item = _connectedUsers.FirstOrDefault(x => x.ConnectionId == id);
            if (item != null)
            {
                _connectedUsers.Remove(item);

                Clients.All.onUserDisconnected(item.EmployeeId, item.Name);
                Clients.All.onlineUsers(_connectedUsers.Count - 1); //send actual number of available users
                GlobalVar.chat.Invoke("OnUserDisconnected", item.EmployeeId);

            }
            return base.OnDisconnected();
        }

        public void PrivateMessage(int toUserId, string message) //etap 1
        {
            var fromUser = _connectedUsers.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);

            GlobalVar.chat.Invoke("PrivateMessage", fromUser.EmployeeId, toUserId, message);
        }

        private async Task CreatePrivateWindow(int fromUserId, int toUserId, string fromUserName, string message) //etap 3 
        {
            Employee toUser = _connectedUsers.FirstOrDefault(x => x.EmployeeId == toUserId);
            Employee fromUser = _connectedUsers.FirstOrDefault(x => x.EmployeeId == fromUserId);

            if (toUser != null && fromUser != null)
            {
                await Clients.Client(toUser.ConnectionId).createNewWindow(fromUser.EmployeeId, fromUserName, message);
            }
        }

        public async Task SendMessage(bool newWindow, int fromUserId, string fromUserName, string message)//metoda potrzebna tylko przy historii!
        {
            //var id = Context.ConnectionId;
            //var toUser = _connectedUsers.FirstOrDefault(x => x.ConnectionId == id);

            DateTime date = DateTime.Now;

            //if (newWindow)
            //{
            //    GlobalVar.chat.Invoke("GetHistory", fromUserId, toUser.EmployeeId, message, GetDateTimeString(date));
            //}

            await Clients.Caller.addMessage(fromUserId, fromUserName, message, GetDateTimeString(date));
        }


        //public async Task AddMessages(List<Message> messages, int fromUserId, int toUserId, string message, string date)
        //{
        //    var fromUser = _connectedUsers.FirstOrDefault(x => x.EmployeeId == fromUserId);
        //    foreach (var msg in messages)
        //    {
        //        await Clients.Caller.addMessage(fromUserId, fromUser.Name, message, date);
        //    }
        //}



        public void SendDisconnectUser(int disconnectUserId)
        {
            var employee = _connectedUsers.FirstOrDefault(x => x.EmployeeId == disconnectUserId);
            if (employee == null) return;
            _connectedUsers.Remove(employee);
            Clients.All.onUserDisconnected(employee.EmployeeId, employee.Name); //odswiezanie listy aktywnych
            Clients.All.onlineUsers(_connectedUsers.Count - 1);
        }

        public void SendConnectUser(int[] ConnectUsersIds)
        {
            var id = Context.ConnectionId;
            var employee = _connectedUsers.FirstOrDefault(x => x.ConnectionId == id);



            foreach (var userId in ConnectUsersIds)
            {
                if (_connectedUsers.Any(x => x.EmployeeId == userId)) _connectedUsers.Add(new Employee
                {
                    EmployeeId = userId
                });
            }


            if (employee != null) Clients.All.onConnected(employee.EmployeeId, _connectedUsers);
            Clients.All.onlineUsers(_connectedUsers.Count - 1);
        }

        public void OnNewUserConnected(int userId, string userName)
        {
            if (_connectedUsers.Count(x => x.EmployeeId == userId) == 0)
            {
                var employee = new Employee { Name = userName, EmployeeId = userId };
                _connectedUsers.Add(employee);

                Clients.All.onNewUserConnected(userId, userName); // send to all except caller client

                Clients.All.onlineUsers(_connectedUsers.Count - 1); //send actual number of available users
            }
        }



        private static string GetDateTimeString(DateTime date)
        {
            if (date.ToShortDateString() == DateTime.Now.ToShortDateString())
                return String.Format("Dzisiaj, {0}", date.ToLongTimeString());
            return String.Format("{0}r., {1}", date.ToString("dd.MM.yyyy"), date.ToLongTimeString());
        }
    }
}