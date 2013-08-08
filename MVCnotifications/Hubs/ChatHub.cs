using System;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading;

namespace MVCnotifications
{
    public class ChatHub : Hub
    {
        public void Send(string name, string message)
        {
            
                Clients.All.addNewMessageToPage(name, message);
            


        }
    }
}