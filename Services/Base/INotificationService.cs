using ScholaAi.Models;

namespace ScholaAi.Services.Base
{
    public interface INotificationService
    {
        Task SendNotification(string senderId, string receiverId, string message, NotificationType type, int? sessionId = null, int? requestId = null);
        Task<List<Notification>> GetUserNotifications(string userId);
    }
}
