namespace Notifications.Base
{
    public interface IEmployee
    {
        string EmployeeId { get; set; }
        string Name { get; set; }
        string ConnectionId { get; set; }
    }
}