using Microsoft.AspNet.SignalR;
using Notifications.Mvc.Models;

namespace Notifications.Mvc.Hubs
{
    public class NoticeHub : Hub
    {
        readonly Message _mes = new Message
        {
            Name = "jakistam",
            Description = "masz nowa wiadomosc"
        };

       
        public void Send()
        {
            Clients.All.addNewMessageToPage(_mes.Name, _mes.Description);
        }
    }
}