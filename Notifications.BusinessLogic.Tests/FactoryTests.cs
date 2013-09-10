//using System;
//using System.Collections.Generic;
//using Notifications.Base;
//using NSubstitute;
//using Xunit.Extensions;

//namespace Notifications.BusiessLogic.Tests
//{
//    internal class FactoryTests
//    {
//        [Theory]
//        [InlineData("Hello", 1, new List<int> { 1, 2, 3 }, DateTime.Now)]
//        [InlineData("wiadomosc", 4, new List<int>() { 5, 0x3, 2 }, new DateTime(1000))]
//        public void Application_BrodcastNotification_should_send_notification_for_each_receivers_to_repository(
//            string content, int senderId, List<int> receiversIds, DateTime date)
//        {
//            //arrange
//            var repository = Substitute.For<IDataRepository>();

//            var factory = new Factory(repository);
//            var application = new Application(factory);a

//            //act
//            factory.AddNotification(new Notification
//            {
//                Content = content,
//                SenderId = senderId,
//                ReceiversIds = receiversIds,
//                Date = date
//            });

//            //assert

//            repository.Received().AddNotification(new Notification
//            {
//                Content = content,
//                SenderId = senderId,
//                ReceiversIds = receiversIds,
//                Date = date
//            });
//        }


//        [Theory]
//        [InlineData("Hello", 1, 3, DateTime.Now)]
//        [InlineData("wiadomosc", 2, 4, DateTime.Now)]
//        public void Application_SendMessage_should_send_notification_for_each_receivers_to_repository(string content,
//            int senderId, int receiverId, DateTime date)
//        {
//            //arrange
//            var repository = Substitute.For<IDataRepository>();

//            var factory = new Factory(repository);
//            var application = new Application(factory);

//            //act
//            factory.AddMessage(new Message
//            {
//                Content = content,
//                SenderId = senderId,
//                ReceiverId = receiverId,
//                Date = date
//            });

//            //assert
//            repository.Received().AddMessage(new Message
//            {
//                Content = content,
//                SenderId = senderId,
//                ReceiverId = receiverId,
//                Date = date
//            });
//        }
//    }
//}

