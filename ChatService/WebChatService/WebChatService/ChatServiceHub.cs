using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using Notifications.Base;
using Notifications.BusiessLogic;
namespace WebChatService
{
    public class ChatServiceHub : Hub
    {
        private static readonly List<User> ConnectedUsers = new List<User>();

        public async Task ConnectUser(string userName, int userId) //polaczenie sie nowego uzytkownika 
        {
            var id = Context.ConnectionId;

            Debug.WriteLine(String.Format("Connect users(start method)= ", ConnectedUsers.Count));
            Debug.WriteLine(String.Format("ConnectUser userName: {0}, userId: {1}", userName, userId));
            if (!ConnectedUsers.Any(u => u.ConnectionId.Any(x => x == id)))
            {
                Dictionary<int, string> connectUsers = new Dictionary<int, string>();

                foreach (var user in ConnectedUsers)
                    connectUsers.Add(user.EmployeeId, user.EmployeeName);

                await Clients.Caller.SendConnectUsers(true, connectUsers);
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
                await  Clients.AllExcept(id).onNewUserConnected(userId, userName);// wyslij pozostalym ze user sie polaczyl
            }
            else
            {
                var user = ConnectedUsers.FirstOrDefault(x => x.EmployeeId == userId);
                if(!user.ConnectionId.Any(x=>x ==id)) user.ConnectionId.Add(id);
            }
            Debug.WriteLine(String.Format("Connect users(end method)= ", ConnectedUsers.Count));
        }

        public void OnUserDisconnected(int employeeId)
        {
            var id = Context.ConnectionId;

            Debug.WriteLine(String.Format("OnUserDisconnected(employeeId: {0}", employeeId));

            var user = ConnectedUsers.FirstOrDefault(x => x.EmployeeId == employeeId);
            user.ConnectionId.Remove(id);
            if (user.ConnectionId.Count == 0) ConnectedUsers.Remove(user);

           

            Clients.All.SendDisconnectUser(user.EmployeeId);
        }

        public override Task OnDisconnected() 
        {
            var id = Context.ConnectionId;

            Debug.WriteLine(String.Format("OnDisconnected(userConnectionId: {1}", id));

            var result = ConnectedUsers.FindAll(user=> user.ConnectionId.Any(x => x == id)).ToList();

            foreach (var b in result)
            {
                ConnectedUsers.Remove(b);
            }

            Dictionary<int, string> connectUsers = new Dictionary<int, string>();

            foreach (var user in ConnectedUsers)
                connectUsers.Add(user.EmployeeId, user.EmployeeName);

            Clients.All.SendConnectUsers(false, connectUsers); //wyslij pozostalym ze wszyscy uzytkownicy z danej app sie rozlaczyli
            
            return base.OnDisconnected();
        }

        //wyslanie wiadomosci 
        public async Task PrivateMessage(int fromUserId, int toUserId, string message)//etap 2
        {
            var toUser = ConnectedUsers.FirstOrDefault(x => x.EmployeeId == toUserId);
            var fromUser = ConnectedUsers.FirstOrDefault(x => x.EmployeeId == fromUserId);

            Debug.WriteLine(String.Format("PrivateMessage(fromUserId: {0},toUserId: {1}, message: {2}) ", fromUserId, toUserId, message));

            foreach (var connId in toUser.ConnectionId)//dla wszystkich okienek odbiorcy wiadomosci
            {
                Debug.WriteLine(String.Format("ConnectionID receiver: {0} ", connId));
                await Clients.Client(connId).CreatePrivateWindow(fromUserId, toUserId, fromUser.EmployeeName, message);

            }

            foreach (var connId in fromUser.ConnectionId)//dla wszystkich okienek nadawcy wiadomosci
            {
                Debug.WriteLine(String.Format("ConnectionID sender: {0} ", connId));
                await Clients.Client(connId).CreatePrivateWindow(toUserId, fromUserId, fromUser.EmployeeName, message);
            }
        }
    }
}