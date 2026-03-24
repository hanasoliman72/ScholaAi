using ScholaAi.Models;
using ScholaAi.Repositories.Base;
using ScholaAi.Services.Base;
using Microsoft.AspNetCore.SignalR;
using ScholaAi.Hubs;
namespace ScholaAi.Services.Notifications
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _repo;
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(INotificationRepository repo, IHubContext<NotificationHub> hubContext)
        {
            _repo = repo;
            _hubContext = hubContext;
        }

        public async Task<List<Notification>> GetUserNotifications(string userId)
        {

            return await _repo.GetUserNotifications(userId);
        }

        public async Task SendNotification(string senderId, string receiverId, string message, NotificationType type, int? sessionId = null, int? requestId = null)
        {
            Notification notif = new Notification
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Message = message,
                Type = type,
                SessionId = sessionId,
                RequestId = requestId
            };

            await _repo.AddNotification(notif);

            await _hubContext.Clients
                .User(receiverId)
                .SendAsync("ReceiveNotification", notif);
        }
    }
}
