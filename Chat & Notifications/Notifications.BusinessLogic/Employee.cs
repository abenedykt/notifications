using Notifications.Base;

namespace Notifications.BusiessLogic
{
    public class Employee : IEmployee
    {
        public string EmployeeId { get; set; }
        public string Name { get; set; }
        public string ConnectionId { get; set; }
    }
}