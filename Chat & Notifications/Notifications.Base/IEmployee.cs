namespace Notifications.Base
{
    public interface IEmployee
    {
        int EmployeeId { get; set; }
        string Name { get; set; }
        string ConnectionId { get; set; }
    }
}