using Notifications.Base;
using Notifications.Mvc.Controllers;
using NSubstitute;
using Xunit;

namespace Notifications.Mvc.Tests
{
    public class HomeControllerTests
    {
        [Fact]
        public void HomeBrodcast_send_messages_to_application()
        {
            //arrange
            var application = Substitute.For<IApplication>();
            var controller = new HomeController(application);

            //act
            controller.Example();

            //assert
            application.ReceivedWithAnyArgs().BrodcastNotification("Hello",new []{1,2,4});
        }
    }
}
