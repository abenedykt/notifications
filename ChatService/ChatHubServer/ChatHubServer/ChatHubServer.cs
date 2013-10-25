using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Notifications.Base;
using Notifications.BusiessLogic;
using Notifications.DataAccessLayer;

namespace WebApplication1
{
    public class ChatHubServer : Hub
    {
        private static readonly List<User> ConnectedUsers = new List<User>();

        public async Task ConnectUser(string userName, int userId) //polaczenie sie nowego uzytkownika 
        {
            var id = Context.ConnectionId;
           

            if (!ConnectedUsers.Any(u => u.ConnectionId.Any(x => x == id)))
            {
                var connectUsers = ConnectedUsers.Select(u => u.EmployeeId).ToList();

                await Clients.Caller.SendConnectUsers(connectUsers);
            }

            if (ConnectedUsers.Count(x => x.EmployeeId == userId) == 0)
            {
                var user = new User
                {
                    EmployeeId = userId,
                    EmployeeName = userName,
                    ConnectionId = new List<string>()
                };
                user.ConnectionId.Add(id);
                ConnectedUsers.Add(user);
                await Clients.AllExcept(id).onNewUserConnected(userId, userName);// wyslij pozostalym ze user sie polaczyl
            }
            else
            {
                var user = ConnectedUsers.FirstOrDefault(x => x.EmployeeId == userId);
                if (!user.ConnectionId.Any(x => x == id)) user.ConnectionId.Add(id);
            }
        }

        public void OnUserDisconnected(int employeeId)
        {
            var id = Context.ConnectionId;

            var user = ConnectedUsers.FirstOrDefault(x => x.EmployeeId == employeeId);
            user.ConnectionId.Remove(id);
            if (user.ConnectionId.Count == 0) ConnectedUsers.Remove(user);

            Clients.AllExcept(id).SendDisconnectUser(user.EmployeeId);
        }

        public override Task OnDisconnected()
        {
            var id = Context.ConnectionId;

            var result = ConnectedUsers.FindAll(user => user.ConnectionId.Any(x => x == id)).ToList();

            foreach (var b in result)
            {
                ConnectedUsers.Remove(b);
            }

            var connectUsers = ConnectedUsers.Select(user => user.EmployeeId).ToList();

            Clients.AllExcept(id).SendConnectUsers(connectUsers); //wyslij pozostalym ze wszyscy uzytkownicy z danej app sie rozlaczyli

            return base.OnDisconnected();
        }

        //wyslanie wiadomosci 
        public async Task PrivateMessage(int fromUserId, int toUserId, string message)//etap 2
        {
            var toUser = ConnectedUsers.FirstOrDefault(x => x.EmployeeId == toUserId);
            var fromUser = ConnectedUsers.FirstOrDefault(x => x.EmployeeId == fromUserId);

            foreach (var connId in toUser.ConnectionId)//dla wszystkich okienek odbiorcy wiadomosci
            {
                await Clients.Client(connId).CreatePrivateWindow(fromUserId, toUserId, fromUser.EmployeeName, message);
            }

            foreach (var connId in fromUser.ConnectionId)//dla wszystkich okienek nadawcy wiadomosci
            {
                await Clients.Client(connId).CreatePrivateWindow(toUserId, fromUserId, fromUser.EmployeeName, message);
            }
        }
    }
}