using Notifications.Base;

namespace Notifications.BusiessLogic
{
    public class RavenStringConnection : IStringConnection
    {
        public string DatabaseUrl { get; set; }
        public string DatabaseName { get; set; }
    }
}
