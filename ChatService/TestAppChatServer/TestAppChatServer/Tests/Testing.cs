using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Moq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using TestAppChatServer.Hubs;
using Xunit;

namespace TestAppChatServer.Tests
{
    public class Testing
    {
        
        //[Fact]
        //public void ConnectNewUser_AddToConnectedUsers()
        //{
        //    TestableChatHub chat = new TestableChatHub();

        //    const string employeeName = "Adam Kanarek";
        //    const string employeeId = "akanarek";

        //    chat.Connect(employeeName, employeeId);

        //    var result = chat.GetLastUser();

        //    var id = chat.Context.ConnectionId;

        //    string id2 = " ";

        //    if (result != null)
        //        id2 = result.ConnectionId;

        //    Assert.Equal(id, id2);
        //}

        //[Fact]
        //public void SendMessage_returnTrue()
        //{
        //    TestableChatHub chat = new TestableChatHub();

        //    const string employeeName = "Adam Kanarek";
        //    const string employeeId = "akanarek";

        //    const string employeeName2 = "Emil Suski";
        //    const string employeeId2 = "esuski";

        //    string message = "hello world!!";

        //    chat.Connect(employeeName, employeeId);
        //    chat.Connect(employeeName2, employeeId2);

        //    var result = chat.SendMessage(employeeId, message);

        //    Assert.True(result);
        //}
        //[Fact]
        //public void SendMessage()
        //{
        //    TestableChatHub chat = new TestableChatHub();
        //    const string employeeName = "Adam Kanarek";
        //    const string employeeId = "akanarek";
        //    const string employeeName2 = "Emil Suski";
        //    const string employeeId2 = "esuski";
        //    string message = "hello world!!";
        //    chat.Connect(employeeName, employeeId);
        //    chat.Connect(employeeName2, employeeId2);
        //    var result = chat.SendMessage(employeeId, message);
        //    Assert.True(result);                  
        //}

        [Fact]
        public void HubsAreMockableViaDynamic()
        {
            var hub = new TestableChatHub();

            const string employeeName = "Adam Kanarek";
            const string employeeId = "akanarek";

            const string employeeName2 = "Emil Suski";
            const string employeeId2 = "esuski";

            string message = "hello world!!";

            hub.Connect(employeeName, employeeId);
            hub.Connect(employeeName2, employeeId2);

            var mockClients = new Mock<IHubCallerConnectionContext>();
            hub.Clients = mockClients.Object;

            bool sendCalled = false;
            bool sendCalled2 = false; 

            dynamic caller = new ExpandoObject();
            dynamic client = new ExpandoObject();

            caller.addMessage = new Action<bool, string, string, string, string, string>((savingx, employeeIdx, employeeNamex, employeeName2x, messagex, datex) =>
            {
                sendCalled = true;
            });
            client.addMessage = new Action<bool, string, string, string, string, string>((savingx2, employeeIdx2, employeeNamex2, employeeName2x2, messagex2, datex2) =>
            {
                sendCalled2 = true;
            });

            mockClients.Setup(m => m.Caller).Returns((ExpandoObject)caller);
            mockClients.Setup(m => m.Client("1234")).Returns((ExpandoObject)client);

            hub.SendMessage(employeeId2, message);

            Assert.True(sendCalled);
            Assert.True(sendCalled2);
        }

        [Fact]
        public void HubsAreMockableViaType()
        {
            var hub = new TestableChatHub();

            const string employeeName = "Adam Kanarek";
            const string employeeId = "akanarek";

            const string employeeName2 = "Emil Suski";
            const string employeeId2 = "esuski";

            string message = "hello world!!";

            hub.Connect(employeeName, employeeId);
            hub.Connect(employeeName2, employeeId2);

            var mockClients = new Mock<IHubCallerConnectionContext>();
            hub.Clients = mockClients.Object;

            var caller = new Mock<IClientContract>();
            var client = new Mock<IClientContract>();
        
            caller.Setup(m => m.addMessage(It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Verifiable();
            client.Setup(m => m.addMessage(It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Verifiable();
            
            mockClients.Setup(m => m.Caller).Returns(caller.Object);
            mockClients.Setup(m => m.Client("1234")).Returns(client.Object);

            hub.SendMessage(employeeId2, message);

            caller.VerifyAll();
            client.VerifyAll();
        }

        public interface IClientContract
        {
            void addMessage(bool saving, string toUserId, string toUserName, string fromUserName, string message, string date);
        }
    }
}