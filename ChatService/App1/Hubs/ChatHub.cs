using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Client;

using Notifications.Base;
using Notifications.BusiessLogic;

namespace App1.Hubs
{
    public class ChatHub : Hub
    {
        private static List<Employee> _connectedUsers = new List<Employee>();

        public ChatHub()
        {
            GlobalVar.chat.On<int, string>("OnNewUserConnected", OnNewUserConnected);
            GlobalVar.chat.On<int>("SendDisconnectUser", SendDisconnectUser);
            GlobalVar.chat.On<bool, Dictionary<int, string>>("SendConnectUsers", SendConnectUsers);
            GlobalVar.chat.On<int, int, string, string>("CreatePrivateWindow", (fromUserId, toUserId, fromUserName, message) => CreatePrivateWindow(fromUserId, toUserId, fromUserName, message));
        }

        public void Connect(string userName, int userId)//polaczenie sie nowego klienta 
        {
            Debug.WriteLine(String.Format("Connect users(start method)= ", _connectedUsers.Count));
            Debug.WriteLine(String.Format("Connect userName: {0}, userId: {1}",  userName, userId));
            var id = Context.ConnectionId;
            Debug.WriteLine(String.Format("Connect connectionID: {0}", id));
            if (_connectedUsers.Count(x => x.EmployeeId == userId) == 0)
            {
                var employee = new Employee { ConnectionId = id, Name = userName, EmployeeId = userId };
                _connectedUsers.Add(employee);

                Clients.Caller.onConnected(userId, _connectedUsers); // send list of active person to caller

                Clients.AllExcept(id).onNewUserConnected(userId, userName); // send to all except caller client

                Clients.All.onlineUsers(_connectedUsers.Count - 1); //send actual number of available users

                GlobalVar.chat.Invoke("ConnectUser", userName, userId);
            }
            else
            {
                _connectedUsers.FirstOrDefault(x => x.EmployeeId == userId).ConnectionId = Context.ConnectionId;
                Clients.Caller.onConnected(userId, _connectedUsers).Wait(); // send list of active person to caller
                Clients.Caller.onlineUsers(_connectedUsers.Count - 1).Wait(); //send actual number of available users
            }
            Debug.WriteLine(String.Format("Connect users(end method)= ", _connectedUsers.Count));
        }

        public override Task OnDisconnected() //w przypadku rozlaczenia sie uzytkownika
        {
            Debug.WriteLine(String.Format("Ondisconnected:Connect users(start method)= ", _connectedUsers.Count));
            var id = Context.ConnectionId;
            Debug.WriteLine(String.Format("OnDisconnected(userConnectionId: {1}", id));

            Thread.Sleep(1000);
            Employee item = _connectedUsers.FirstOrDefault(x => x.ConnectionId == id);

            if (item != null)
            {
                GlobalVar.chat.Invoke("OnUserDisconnected", item.EmployeeId);
            }
            Debug.WriteLine(String.Format("Ondisconnected:Connect users(end method)= ", _connectedUsers.Count));
            return base.OnDisconnected();
        }

        public void PrivateMessage(int toUserId, string message) //etap 1
        {
            var fromUser = _connectedUsers.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);

            Debug.WriteLine(String.Format("PrivateMessage(toUserId: {0}, message {1}", toUserId, message));

            GlobalVar.chat.Invoke("PrivateMessage", fromUser.EmployeeId, toUserId, message);
        }

        private async Task CreatePrivateWindow(int fromUserId, int toUserId, string fromUserName, string message) //etap 3 
        {
            Employee toUser = _connectedUsers.FirstOrDefault(x => x.EmployeeId == toUserId);
            Employee fromUser = _connectedUsers.FirstOrDefault(x => x.EmployeeId == fromUserId);

            Debug.WriteLine(String.Format("CreatePrivateWindow(fromUserId: {0}, toUserId: {1}, fromUserName: {2},message: {3}", fromUserId, toUserId, fromUserName, message));

            if (toUser != null && fromUser != null)
            {
                await Clients.Client(toUser.ConnectionId).createNewWindow(fromUser.EmployeeId, fromUserName, message);
            }
        }

        public async Task SendMessage(bool newWindow, int fromUserId, string fromUserName, string message)//metoda potrzebna tylko przy historii!
        {
            DateTime date = DateTime.Now;

            Debug.WriteLine(String.Format("SendMessage(newWindow: {1}, fromUserId: {1},fromUserName: {2},message: {3}", newWindow, fromUserId, fromUserName, message));
            
            await Clients.Caller.addMessage(fromUserId, fromUserName, message, GetDateTimeString(date));
        }

        public void SendDisconnectUser(int disconnectUserId)
        {
            var employee = _connectedUsers.FirstOrDefault(x => x.EmployeeId == disconnectUserId);
            if (employee == null) return;
            Debug.WriteLine(String.Format("SendDisconnectUser(disconnectUserId: {0}", disconnectUserId));
            _connectedUsers.Remove(employee);
            Clients.All.onUserDisconnected(employee.EmployeeId, employee.Name).Wait(); //odswiezanie listy aktywnych
            Clients.All.onlineUsers(_connectedUsers.Count - 1).Wait();
        }

        public void SendConnectUsers(bool isFirstLogin, Dictionary<int, string> ConnectUsersIds)
        {
            var id = Context.ConnectionId;
            var employee = _connectedUsers.FirstOrDefault(x => x.ConnectionId == id);

            Debug.WriteLine(String.Format("SendConnectUsers(isFirstLogin: {0}, )", isFirstLogin));

            foreach (var userId in ConnectUsersIds)
            {
                Debug.WriteLine(String.Format("ConnectUsersId[" + userId.Key + "]: ", userId.Value));

                if (!_connectedUsers.Any(x => x.EmployeeId == userId.Key)) _connectedUsers.Add(new Employee
                {
                    EmployeeId = userId.Key,
                    Name = userId.Value
                });
            }
            if (isFirstLogin) Clients.Caller.onConnected(_connectedUsers);
            else Clients.All.onConnected(_connectedUsers);
            Clients.All.onlineUsers(_connectedUsers.Count - 1);
            
        }

        public void OnNewUserConnected(int userId, string userName)
        {
            Debug.WriteLine(String.Format("OnNewUserConnected(userId: {0}, userName: {1}", userId, userName));

            if (_connectedUsers.Count(x => x.EmployeeId == userId) == 0)
            {
                var employee = new Employee { Name = userName, EmployeeId = userId };
                _connectedUsers.Add(employee);

                Clients.All.onNewUserConnected(userId, userName); // send to all except caller client

                Clients.All.onlineUsers(_connectedUsers.Count - 1); //send actual number of available users        
            }
            Debug.WriteLine(String.Format("OnNewUserConnected:Connect users: {0}", _connectedUsers.Count));    

        }

        private static string GetDateTimeString(DateTime date)
        {
            if (date.ToShortDateString() == DateTime.Now.ToShortDateString())
                return String.Format("Dzisiaj, {0}", date.ToLongTimeString());
            return String.Format("{0}r., {1}", date.ToString("dd.MM.yyyy"), date.ToLongTimeString());
        }
    }
}