using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR.Client.Hubs;
using Microsoft.AspNet.SignalR.Client;

namespace App2
{
    public class GlobalVar
    {
        public static HubConnection connection;
        public static IHubProxy chat;
    }
}