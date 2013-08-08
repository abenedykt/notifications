using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading;
using MVCnotifications.Models;

namespace MVCnotifications.Hubs
{
    public class NoticeHub : Hub
    {
        Message mes = new Message
        {
            Name = "jakistam",
            Description = "masz nowa wiadomosc"
        };

       
        public void Send()
        {
            Clients.All.addNewMessageToPage(mes.Name, mes.Description);
            
        }


    }
}