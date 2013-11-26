using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Moq;
using Notifications.BusiessLogic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Web;
using TestAppChatServer.Hubs;
using Xunit;
using Xunit.Extensions;

namespace TestAppChatServer.Tests
{
    public class Testing
    {    
       //HubsAreMockableViaDynamic
        [Fact]
        public void ViaDynamic_AddText_In_Success_Was_Called_Correctly() 
        {
            var hub = new TestableChatHub();
            
            var mockClients = new Mock<IHubCallerConnectionContext>();
            hub.Clients = mockClients.Object;

            bool sendCalled = false;
            
            dynamic caller = new ExpandoObject();

            caller.addText = new Action(() => 
            {
                sendCalled = true;
            });
            
            mockClients.Setup(m => m.Caller).Returns((ExpandoObject)caller);
            hub.Success();

            Assert.True(sendCalled);
        }

        [Theory]
        [InlineData("Adam Kanarek", "akanarek", "hello world!!")]
        [InlineData("Wojciech Kondrat", "wkondrat", "witaj swiecie!")]
        public void ViaDynamic_AddMessage_In_SendMessage_Was_Called_Correctly(string employeeName, string employeeId, string message)
        {
            var hub = new TestableChatHub();           
           
            hub.Connect(employeeName, employeeId);
           
            var mockClients = new Mock<IHubCallerConnectionContext>();
            hub.Clients = mockClients.Object;

            bool sendCalled = false;
            bool sendCalled2 = false;

            dynamic caller = new ExpandoObject();
            dynamic client = new ExpandoObject();

            caller.addMessage = new Action<bool, string, string, string, string, string>((saving, emplId, emplName1, emplName2, msg, date) =>
            {
                sendCalled = true;
            });
            client.addMessage = new Action<bool, string, string, string, string, string>((saving, emplId, emplName1, emplName2, msg, date) =>
            {
                sendCalled2 = true;
            });

            mockClients.Setup(m => m.Caller).Returns((ExpandoObject)caller);
            mockClients.Setup(m => m.Client(hub.connectionId)).Returns((ExpandoObject)client);

            hub.SendMessage(employeeId, message);

            Assert.True(sendCalled);
            Assert.True(sendCalled2);
        }

        [Theory]
        [InlineData("Maria Bosacka", "mbosacka")]
        [InlineData("Wojciech Kondrat", "wkondrat")]
        public void ViaDynamic_OnConnected_In_Connect_Was_Called_Correctly_User_Connect_First_Time(string employeeName, string employeeId)
        {
            var hub = new TestableChatHub();

            hub.DeleteAllConnectedUsers();

            var mockClients = new Mock<IHubCallerConnectionContext>();
            hub.Clients = mockClients.Object;

            bool sendCalled = false;

            dynamic all = new ExpandoObject();

            all.onConnected = new Action<List<Employee>>(connectedUsers =>
            {
                sendCalled = true;
            });

            mockClients.Setup(m => m.All).Returns((ExpandoObject)all);
            
            hub.Connect(employeeName, employeeId);
            
            Assert.True(sendCalled);
        }

        [Theory]
        [InlineData("Maria Bosacka", "mbosacka")]
        [InlineData("Wojciech Kondrat", "wkondrat")]
        public void ViaDynamic_OnConnected_In_Connect_Was_Called_Correctly_User_Connect_Again(string employeeName, string employeeId)
        {
            var hub = new TestableChatHub();

            hub.DeleteAllConnectedUsers();

            var mockClients = new Mock<IHubCallerConnectionContext>();
            hub.Clients = mockClients.Object;

            bool sendCalled = false;
            bool sendCalled2 = false;

            dynamic all = new ExpandoObject();
            dynamic caller = new ExpandoObject();

            all.onConnected = new Action<List<Employee>>(connectedUsers =>
            {
                sendCalled = true;
            });

            caller.onConnected = new Action<List<Employee>>(connectedUsers =>
            {
                sendCalled2 = true;
            });

            mockClients.Setup(m => m.All).Returns((ExpandoObject)all);
            mockClients.Setup(m => m.Caller).Returns((ExpandoObject)caller);

            hub.Connect(employeeName, employeeId);
            hub.Connect(employeeName, employeeId);

            Assert.True(sendCalled);
            Assert.True(sendCalled2);
        }

        [Theory]
        [InlineData("Maria Bosacka", "mbosacka")]
        [InlineData("Wojciech Kondrat", "wkondrat")]
        public void ViaDynamic_OnUserDisconnected_In_OnDisconnected_Was_Called_Correctly(string employeeName, string employeeId)
        {
            var hub = new TestableChatHub();

            hub.Connect(employeeName, employeeId);

            var mockClients = new Mock<IHubCallerConnectionContext>();
            hub.Clients = mockClients.Object;

            bool sendCalled = false;

            dynamic all = new ExpandoObject();

            all.onUserDisconnected = new Action<string,List<Employee>>((emplId, connectedUsers) =>
            {
                sendCalled = true;
            });

            mockClients.Setup(m => m.All).Returns((ExpandoObject)all);

            hub.OnDisconnected();

            Assert.True(sendCalled);
        }

        [Theory]
        [InlineData("Maria Bosacka", "mbosacka")]
        [InlineData("Wojciech Kondrat", "wkondrat")]
        public async void ViaDynamic_AddMessage_In_GetHistory_Was_Called_Correctly(string employeeName, string employeeId)
        {
            var hub = new TestableChatHub();

            hub.Connect(employeeName, employeeId);

            var mockClients = new Mock<IHubCallerConnectionContext>();
            hub.Clients = mockClients.Object;

            bool sendCalled = false;
            bool sendCalled2 = false;

            dynamic caller = new ExpandoObject();

            caller.addMessage = new Action<bool, string, string, string, string, string>((saving, emplId, emplName1, emplName2, msg, date) =>
            {
                sendCalled = true;
            });

            mockClients.Setup(m => m.Caller).Returns((ExpandoObject)caller);

            await hub.GetHistory(employeeId);

            Assert.True(sendCalled);
            Assert.True(sendCalled2);
        }

        ////HubsAreMockableViaType
        [Theory]
        [InlineData("Adam Kanarek", "akanarek", "hello world!!")]
        [InlineData("Wojciech Kondrat", "wkondrat", "witaj swiecie!")]
        public void ViaType_AddMessage_Was_Called_Correctly(string employeeName, string employeeId, string message)
        {
            var hub = new TestableChatHub();

            hub.Connect(employeeName, employeeId);

            var mockClients = new Mock<IHubCallerConnectionContext>();
            hub.Clients = mockClients.Object;

            var caller = new Mock<IClientContract>();
            var client = new Mock<IClientContract>();

            caller.Setup(m => m.addMessage(It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Verifiable();
            client.Setup(m => m.addMessage(It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Verifiable();

            mockClients.Setup(m => m.Caller).Returns(caller.Object);
            mockClients.Setup(m => m.Client(hub.connectionId)).Returns(client.Object);

            hub.SendMessage(employeeId, message);

            caller.VerifyAll();
            client.VerifyAll();
        }

        [Theory]
        [InlineData("Maria Bosacka", "mbosacka")]
        [InlineData("Wojciech Kondrat", "wkondrat")]
        public void ViaType_OnConnected_In_Connect_Was_Called_Correctly_User_Connect_First_Time(string employeeName, string employeeId)
        {
            var hub = new TestableChatHub();

            hub.DeleteAllConnectedUsers();

            var mockClients = new Mock<IHubCallerConnectionContext>();
            hub.Clients = mockClients.Object;

            var all = new Mock<IClientContract>();

            all.Setup(m => m.onConnected(It.IsAny<List<Employee>>())).Verifiable();
            mockClients.Setup(m => m.All).Returns(all.Object);
            hub.Connect(employeeName, employeeId);
            all.VerifyAll();
        }

        [Theory]
        [InlineData("Maria Bosacka", "mbosacka")]
        [InlineData("Wojciech Kondrat", "wkondrat")]
        public void ViaType_OnConnected_In_Connect_Was_Called_Correctly_User_Connect_Again(string employeeName, string employeeId)
        {
            var hub = new TestableChatHub();

            hub.DeleteAllConnectedUsers();

            var mockClients = new Mock<IHubCallerConnectionContext>();
            hub.Clients = mockClients.Object;

            var all = new Mock<IClientContract>();
            var caller = new Mock<IClientContract>();

            all.Setup(m => m.onConnected(It.IsAny<List<Employee>>())).Verifiable();
            caller.Setup(m => m.onConnected(It.IsAny<List<Employee>>())).Verifiable();

            mockClients.Setup(m => m.All).Returns(all.Object);
            mockClients.Setup(m => m.Caller).Returns(caller.Object);
            hub.Connect(employeeName, employeeId);
            hub.Connect(employeeName, employeeId);

            all.VerifyAll();
            caller.VerifyAll();
        }

        [Theory]
        [InlineData("Maria Bosacka", "mbosacka")]
        [InlineData("Wojciech Kondrat", "wkondrat")]
        public void ViaType_OnUserDisconnected_In_OnDisconnected_Was_Called_Correctly(string employeeName, string employeeId)
        {
            var hub = new TestableChatHub();

            hub.Connect(employeeName, employeeId);

            var mockClients = new Mock<IHubCallerConnectionContext>();
            hub.Clients = mockClients.Object;

            var all = new Mock<IClientContract>();

            all.Setup(m => m.onUserDisconnected(It.IsAny<string>(),It.IsAny<List<Employee>>())).Verifiable();
            
            mockClients.Setup(m => m.All).Returns(all.Object);

            hub.OnDisconnected();

            all.VerifyAll();  
        }

        [Theory]
        [InlineData("Maria Bosacka", "mbosacka")]
        [InlineData("Wojciech Kondrat", "wkondrat")]
        public async void ViaType_AddMessage_In_GetHistory_Was_Called_Correctly(string employeeName, string employeeId)
        {
            var hub = new TestableChatHub();

            hub.Connect(employeeName, employeeId);

            var mockClients = new Mock<IHubCallerConnectionContext>();
            hub.Clients = mockClients.Object;

            var caller = new Mock<IClientContract>();
            

            caller.Setup(m => m.addMessage(It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Verifiable();

            mockClients.Setup(m => m.Caller).Returns(caller.Object);

            await hub.GetHistory(employeeId);

            caller.VerifyAll();
        }

        public interface IClientContract
        {
            void addMessage(bool isSaving, string toUserId, string toUserName, string fromUserName, string message, string date);
            void onConnected(List<Employee> connectedUsers);
            void onUserDisconnected(string employeeId, List<Employee> connectedUsers);
        }
    }
}