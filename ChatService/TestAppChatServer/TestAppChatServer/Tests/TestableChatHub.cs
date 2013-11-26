using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Configuration;
using Microsoft.AspNet.SignalR.Hubs;
using Moq;
using Notifications.BusiessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestAppChatServer.Hubs;
using Xunit;

namespace TestAppChatServer.Tests
{
    public class TestableChatHub : ChatHub
    {
        public IConfigurationManager _config;
        public StateChangeTracker tracker = new StateChangeTracker();
        public DefaultDependencyResolver resolver = new DefaultDependencyResolver();
        public Mock<IConnection> mockConnection = new Mock<IConnection>();
        public Mock<IPrincipal> mockUser = new Mock<IPrincipal>();
        public IHubPipelineInvoker _pipelineInvoker ;
        public Mock<IRequest> mockRequest = new Mock<IRequest>();

        public string connectionId = "12345678";
        string hubName = "ChatHub";
       
        public TestableChatHub()
        {           
            _config = resolver.Resolve<IConfigurationManager>();            

            mockRequest.Setup(r => r.User).Returns(mockUser.Object);

            _pipelineInvoker = resolver.Resolve<IHubPipelineInvoker>();
         
           Clients = new HubConnectionContext(_pipelineInvoker, mockConnection.Object, hubName, connectionId, tracker);
           Context = new HubCallerContext(mockRequest.Object, connectionId);
        }

        public void DeleteAllConnectedUsers()
        {
            ConnectedUsers = new List<Employee>();
        }
    }

}