using System;
using System.Linq;
using Notifications.Base;
using NSubstitute;
using Xunit.Extensions;

namespace Notifications.BusiessLogic.Tests
{

    internal class FactoryTests
    {
        [Theory]
        [InlineData("wiadomosc", 4, new int[] { 5, 3, 2 }, 2011, 11, 11, 12, 12, 54)]
        public void Application_BrodcastNotification_should_send_notification_for_each_receivers_to_repository(
            string content, int senderId, int[] receiversIds, int years, int months, int days, int hours, int minutes, int seconds)
        {
            //arrange
            var repository = Substitute.For<IDataRepository>();
            var factory = new Factory(repository);
            var application = new ChatApplication(factory);

            var receivers = receiversIds.ToList();

            var date = new DateTime(years, months, days, hours, minutes, seconds);

            //act
            application.BrodcastNotification(content, senderId, receivers, date);

            //assert
            repository.Received().AddNotification(new Notification
            {
                Content = content,
                SenderId = senderId,
                ReceiversIds = receivers,
                Date = date
            });
        }

        [Theory]
        [InlineData("Hello", 1, 3, 2011, 11, 11, 12, 12, 54)]
        public void Application_SendMessage_should_send_notification_for_each_receivers_to_repository(string content,
            int senderId, int receiverId, int years, int months, int days, int hours, int minutes, int seconds)
        {
            //arrange
            var repository = Substitute.For<IDataRepository>();

            var factory = new Factory(repository);
            var application = new ChatApplication(factory);

            var date = new DateTime(years, months, days, hours, minutes, seconds);
            //act
            application.SendMessage(content, senderId, receiverId, date);

            //assert
            repository.Received().AddMessage(new Message
            {
                Content = content,
                SenderId = senderId,
                ReceiverId = receiverId,
                Date = date
            });
        }
    }
}