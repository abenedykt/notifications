﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Notifications.Base;
using Notifications.BusiessLogic;


namespace ChatServerApp
{
    public class ChatHub : Hub
    {
        private static readonly List<Employee> ConnectedUsers = new List<Employee>();
        private readonly IChatApplication _application;
        private readonly bool _saving;


        public ChatHub()
        {
            //_saving = false;

            //if (_saving)
            //{
            //    var mongoConnection = new MongoStringConnection
            //    {
            //        DatabaseName = "chat",
            //        DatabaseUrl = "mongodb://emp:12345@localhost/chat"
            //    };
                //_application = new ChatApplication(new Factory(new MongoRepository(mongoConnection)));
            //}          
        }

        public void Success()
        {
            Clients.Caller.addText();
        }

        public void Connect(string userName, string userId)
        {
           

            var id = Context.ConnectionId;

           
            
            if (ConnectedUsers.Count(x => x.EmployeeId == userId) == 0)
            {
                
                var employee = new Employee { ConnectionId = id, Name = userName, EmployeeId = userId };
                ConnectedUsers.Add(employee);

                if(_saving) _application.AddEmployee(employee);

                Clients.All.onConnected(ConnectedUsers).Wait();
            }
            else
            {
                
                var user = ConnectedUsers.FirstOrDefault(x => x.EmployeeId == userId);
                if (user != null)
                    user.ConnectionId = Context.ConnectionId;
                Clients.Caller.onConnected(ConnectedUsers).Wait();
            }
        }
       
        public override Task OnDisconnected()
        {
            var id = Context.ConnectionId;
            

            Thread.Sleep(1000);
            var item = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == id);
            if (item != null)
            {
                
                ConnectedUsers.Remove(item);
                Clients.All.onUserDisconnected(item.EmployeeId, ConnectedUsers).Wait();
            }
            return base.OnDisconnected();
        }

        public async Task SendMessage(string toUserId, string message)
        {
            
            
                var fromUserId = Context.ConnectionId;

                var toUser = ConnectedUsers.FirstOrDefault(x => x.EmployeeId == toUserId);
                var fromUser = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == fromUserId);

                var date = DateTime.Now;


                if (toUser != null && fromUser != null)
                {
                    if (_saving) _application.SendMessage(message, fromUser.EmployeeId, toUserId, date);

                    await Clients.Caller.addMessage(_saving, toUserId, toUser.Name, "Ja", message, date.ToShortTimeString());
                    await Clients.Client(toUser.ConnectionId).addMessage(_saving, fromUser.EmployeeId, fromUser.Name, fromUser.Name, message, date.ToShortTimeString());
                }
           
        }

        public async Task GetHistory(string toUserId)
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

        public Employee GetLastUser()
        {
            var index = ConnectedUsers.Count-1;

            return index>-1 ? ConnectedUsers[index] : null;
        }
    }    
}