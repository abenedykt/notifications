﻿using System;
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

namespace WebChatService
{
    public class ChatServiceHub : Hub
    {
        //private readonly IChatApplication _application;

        private static readonly List<User> ConnectedUsers = new List<User>();
        IHubContext context = GlobalHost.ConnectionManager.GetHubContext<ChatServiceHub>();
       
        public void ConnectUser(string userName, int userId) //polaczenie sie nowego uzytkownika 
        {
            var id = Context.ConnectionId;
            Debug.WriteLine("Id App dla {0} = {1}", userName, id); //to pozniej usun

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
                context.Clients.AllExcept(id).onNewUserConnected(userId, userName);// wyslij pozostalym ze user sie polaczyl
            }
            else
            {
                var user = ConnectedUsers.FirstOrDefault(x => x.EmployeeId == userId);
                if(!user.ConnectionId.Any(x=>x ==id)) user.ConnectionId.Add(id);
            }
        }

        public void OnUserDisconnected(int employeeId)
        {
            var id = Context.ConnectionId;

            var user = ConnectedUsers.FirstOrDefault(x => x.EmployeeId == employeeId);
            user.ConnectionId.Remove(id);
            if (user.ConnectionId.Count == 0) ConnectedUsers.Remove(user);

            context.Clients.AllExcept(id).SendDisconnectUser(user.EmployeeId);
        }

        public override Task OnDisconnected() 
        {
            var id = Context.ConnectionId;

            var result = ConnectedUsers.FindAll(user=> user.ConnectionId.Any(x => x == id)).ToList();

            foreach (var b in result)
            {
                ConnectedUsers.Remove(b);
            }

            var connectUsers = ConnectedUsers.Select(user => user.EmployeeId).ToList();

            Clients.AllExcept(id).SendConnectUsers(connectUsers); //wyslij pozostalym ze wszyscy uzytkownicy z danej app sie rozlaczyli

            return base.OnDisconnected();
        }

        //wyslanie wiadomosci 
        public async Task PrivateMessage(int fromUserId, int toUserId, string message)
        {

            var toUser = ConnectedUsers.FirstOrDefault(x => x.EmployeeId == toUserId);
            var fromUser = ConnectedUsers.FirstOrDefault(x => x.EmployeeId == fromUserId);

            foreach (var connId in toUser.ConnectionId)//dla wszystkich okienek odbiorcy wiadomosci
            {
                await Clients.Client(connId).CreatePrivateWindow(fromUserId, toUserId, fromUser.EmployeeName, message);

            }

            foreach (var connId in fromUser.ConnectionId)//dla wszystkich okienek nadawcy wiadomosci
            {
                await Clients.Client(connId).CreatePrivateWindow(toUserId, fromUserId,fromUser.EmployeeName, message);
            }

        }

        public async Task SendMessage(int toUserId, int fromUserId, string fromUserName, string message, string date)
        {

            var fromUser = ConnectedUsers.FirstOrDefault(x => x.EmployeeId == fromUserId);

            foreach (var connId in fromUser.ConnectionId)
            {
                await Clients.Client(connId).AddMessage(toUserId, fromUserId, fromUserName, message, date);
            }
        }

    }
}