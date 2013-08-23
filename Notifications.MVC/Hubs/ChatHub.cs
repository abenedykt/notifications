using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Notifications.BusiessLogic;
using Notifications.Base;
using System.Web.Mvc;

namespace Notifications.Mvc.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IApplication _application;
        public ChatHub(IApplication application)
        {
            _application = application;
        }

        static List<Employee> ConnectedUsers = new List<Employee>();

        public void Connect(string userName, int userId)
        {
            var id = Context.ConnectionId;

            if (ConnectedUsers.Count(x => x.ConnectionId == id) == 0)
            {
                ConnectedUsers.Add(new Employee { ConnectionId = id, Name = userName, EmployeeId= userId });

                // send list of active person to caller
                Clients.Caller.onConnected(id, userName, ConnectedUsers);

                // send to all except caller client
                Clients.AllExcept(id).onNewUserConnected(id, userName);
             
                //send actual number of available users
                Clients.All.onlineUsers(ConnectedUsers.Count-1);
            }
        }

        public override Task OnDisconnected()
        {
            var item = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            if (item != null)
            {
                ConnectedUsers.Remove(item);

                var id = Context.ConnectionId;
                Clients.All.onUserDisconnected(id, item.Name);
            }

            //send actual number of available users
            Clients.All.onlineUsers(ConnectedUsers.Count - 1);
            
            return base.OnDisconnected();
        }

        public void SendPrivateMessage(string toUserId, string message)
        {
            string fromUserId = Context.ConnectionId;

            var toUser = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == toUserId);
            var fromUser = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == fromUserId);

            DateTime date = DateTime.Now;

            if (toUser != null && fromUser != null)
            {
                // send to 
                Clients.Client(toUserId).sendPrivateMessage(fromUserId, fromUser.Name, message, date.ToLongTimeString());

                // send to caller user
                Clients.Caller.sendPrivateMessage(toUserId, fromUser.Name, message, date.ToLongTimeString());


                //_application.SendMessage(message, fromUser.EmployeeId, toUser.EmployeeId, date);


            }
        }
 
        public async Task Broadcasting(string[] users, string notification, string userName)
        {
            
            foreach (var item in users) 
                await this.Groups.Add(item, "grupa");  
     
            await Clients.Group("grupa").sendNotificationBroadcast(notification, userName);

            foreach (var item in users) 
                await   this.Groups.Remove(item, "grupa");  
        }

     
    }
}