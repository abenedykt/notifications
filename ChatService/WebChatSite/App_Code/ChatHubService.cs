using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using Notifications.Base;

public class ChatHubService : Hub
{
    private readonly IChatApplication _application;

    private List<User> ConnectedUsers = new List<User>();

    public void ConnectUser(string userName, int userId) //polaczenie sie nowego uzytkownika 
    {
        string id = Context.ConnectionId;

        if (ConnectedUsers.Count(x => x.EmployeeId == userId) == 0)
        {
            var user = new User
            {
                EmployeeId = userId,
                ConnectionId = new List<string>()
            };
            user.ConnectionId.Add(id);
            ConnectedUsers.Add(user);
            //Clients.AllExcept(id).onNewUserConnected(userId, userName); wyslij pozostalym ze user sie polaczyl
        }
        else
        {
            var user = ConnectedUsers.FirstOrDefault(x => x.EmployeeId == userId);
            user.ConnectionId.Add(id);
        }
    }

    //rozlaczenie sie jednego uzytkownika
    public void OnUserDisconnected(int employeeId)
    {
        var user = ConnectedUsers.FirstOrDefault(x => x.EmployeeId == employeeId);
        ConnectedUsers.Remove(user);

        //wyslij innym ze user sie rozlaczyl
    }

    public override Task OnDisconnected() //rozlaczenie sie calej app ?
    {
        var id = Context.ConnectionId;

        foreach (
            var connectedUser in ConnectedUsers.Where(connectedUser => connectedUser.ConnectionId.Any(x => x == id)))
        {
            if (connectedUser.ConnectionId.Count <= 1) ConnectedUsers.Remove(connectedUser);
            else connectedUser.ConnectionId.Remove(id);
        }

        var connectUsers = ConnectedUsers.Select(user => user.EmployeeId).ToList();

       // Clients.AllExcept(id).sendConnectUsers(connectUsers); wyslij pozostalym ze wszyscy uzytkownicy z danej app sie rozlaczyli
        
        return base.OnDisconnected();
    }

    //wyslanie wiadomosci 

}




