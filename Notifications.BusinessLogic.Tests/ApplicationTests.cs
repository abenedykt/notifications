using Notifications.Base;
using NSubstitute;
using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Extensions;

namespace Notifications.BusiessLogic.Tests
{
    public class ApplicationTests
    {
        [Theory]
        [InlineData("Hello", 1, new List<int> { 1, 2, 3 }, DateTime.Now)]
        [InlineData("wiadomosc", 4, new List<int> { 5, 3, 2 }, new DateTime(1000))]
        public void Application_BrodcastNotification_should_send_notification_for_each_receivers_to_factory(string content, int senderId, List<int> receiversIds, DateTime date)
        {
            //arrange
            var factory = Substitute.For<IFactory>();
            var application = new Application(factory);

            //act
            application.BrodcastNotification(content, senderId, receiversIds, date);

            //assert

            factory.Received().AddNotification(new Notification
                {
                    Content = content,
                    SenderId = senderId,
                    ReceiversIds = receiversIds,
                    Date = date
                });
        }

        [Theory]
        [InlineData("Hello", 1, 3, DateTime.Now)]
        [InlineData("wiadomosc", 2, 4, DateTime.Now)]
        public void Application_SendMessage_should_send_message_to_factory(string content, int senderId, int receiverId, DateTime date)
        {
            //arrange
            var factory = Substitute.For<IFactory>();
            var application = new Application(factory);

            //act
            application.SendMessage(content, senderId, receiverId, date);

            //assert
            factory.Received().AddMessage(new Message
            {
                Content = content,
                Date = date,
                SenderId = senderId,
                ReceiverId = receiverId
            });
        }

       
    }

}