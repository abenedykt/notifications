using System.Web.Http;

namespace Notifications.Mvc.Controllers
{
    public class ChatController : ApiController
    {
        [HttpGet]
        public string[] GetLastMessages()
        {
            return new[]
            {
                "test1",
                "test2",
                "test3"
            };

            //https://github.com/abenedykt/pilka/blob/master/Pilka/Pilka/Scripts/ControllerOsoby.js
        }
    }
}
