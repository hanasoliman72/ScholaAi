using Microsoft.EntityFrameworkCore;
using ScholaAi.Models;
using ScholaAi.Repositories.Base;

namespace ScholaAi.Repositories.Notification
{
    public class NotificationRepository : INotificationRepository
    {
         private readonly DBcontext _context;
        public NotificationRepository(DBcontext context)
        {
            _context = context;
        }

        public async Task AddNotification(Models.Notification notification)
        {
            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();
           
        }

        public async Task<List<Models.Notification>> GetUserNotifications(string userId)
        {
             return await _context.Notifications.Where(n=> n.ReceiverId == userId)
                .OrderByDescending(n=>n.CreatedAt).ToListAsync();
  
        }

        public async Task MarkAsRead(int notificationId)
        {
            var notfi = await _context.Notifications.FindAsync(notificationId);
            if (notfi != null)
            {
                notfi.IsRead = true;
                _context.SaveChanges();
            }
        }
    }
}
