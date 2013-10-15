using Notifications.Base;

namespace Notifications.BusiessLogic
{
    public class Employee : IEmployee
    {
        public int EmployeeId { get; set; }
        public string Name { get; set; }
        public string ConnectionId { get; set; }
    }
}