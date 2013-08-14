using Microsoft.AspNet.SignalR;

namespace Notifications.Mvc.Hubs
{
    public class ChatHub : Hub
    {
        public void Send(string name, string message)
        {
            
                Clients.All.addNewMessageToPage(name, message);
            


        }
    }
}