using Notifications.Base;

namespace Notifications.BusiessLogic
{
    public class RavenConnection : IDatabaseConnection
    {
        public string DatabaseUrl { get; set; }
        public string DatabaseName { get; set; }
    }
}
