namespace Notifications.Base
{
    public interface IApplication
    {
        void BrodcastNotification(string text, int[] recipientsIDs);
    }
}