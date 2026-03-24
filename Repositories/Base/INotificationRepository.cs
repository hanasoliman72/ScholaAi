using ScholaAi.Models;

namespace ScholaAi.Repositories.Base
{
    public interface INotificationRepository
    {
        Task AddNotification(Models.Notification notification);
        Task<List<Models.Notification>> GetUserNotifications(string userId);
        Task MarkAsRead(int notificationId);
    }
}
