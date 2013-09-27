namespace Notifications.Base
{
    public interface IDatabaseConnection
    {
        string DatabaseUrl { get; set; }
        string DatabaseName { get; set; }
    }
        
    
}
