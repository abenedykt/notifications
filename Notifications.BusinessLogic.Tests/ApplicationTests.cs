using Notifications.Base;
using NSubstitute;
using Xunit;

namespace Notifications.BusiessLogic.Tests
{
    public class ApplicationTests
    {
        [Fact]
        public void BrodcastNotofication_should_send_notification_to_each_recepient()
        {
            //arrange
            var repository = Substitute.For<IDataRepository>();
            var application = new Application(repository);
            
            //act
            application.BrodcastNotification("Hello",new []{1,2,3,4,5});

            //assert

            //repository.Received().AddNotification(new Notification
            //{
            //    Content = "Hello",
            //    Date = DateTime.Now,
            //    ReceiverId = 1,
            //    SenderId = 1
            //});

            //repository.Received().AddNotification(new Notification
            //{
            //    Content = "Hello",
            //    Date = DateTime.Now,
            //    ReceiverId = 2,
            //    SenderId = 1
            //});

            //repository.Received().AddNotification(new Notification
            //{
            //    Content = "Hello",
            //    Date = DateTime.Now,
            //    ReceiverId = 3,
            //    SenderId = 1
            //});

            //repository.Received().AddNotification(new Notification
            //{
            //    Content = "Hello",
            //    Date = DateTime.Now,
            //    ReceiverId = 4,
            //    SenderId = 1
            //});

            //repository.Received().AddNotification(new Notification
            //{
            //    Content = "Hello",
            //    Date = DateTime.Now,
            //    ReceiverId = 5,
            //    SenderId = 1
            //});
            //w repozytorium powinny się pojawić powiadomienia dla kazdego z odbiotrców
        }
    }

}