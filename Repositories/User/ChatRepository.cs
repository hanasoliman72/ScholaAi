using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ScholaAi.Models;
using ScholaAi.DTOs.Chat;
using ScholaAi.Repositories.Base;

namespace ScholaAi.Repositories.User
{
    public class ChatRepository : IChatRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly DBcontext _context;

        public ChatRepository(DBcontext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async  Task<ChatMessage> AddAsync(ChatMessage message)
        {
            _context.ChatMessages.Add(message);
            await _context.SaveChangesAsync();
            return message;
        }

        public async Task<List<ChatMessage>> GetChatHistoryAsync(string userId1, string userId2)
        {
            var messages = await _context.ChatMessages
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .Where(m => (m.SenderId == userId1 && m.ReceiverId == userId2) ||
                            (m.SenderId == userId2 && m.ReceiverId == userId1))
                .OrderBy(m => m.SentAt)
                .ToListAsync();

            return messages;
        }

        public async Task<List<ConversationSummaryDto>> GetUserConversationsAsync(string userId)
        {
            // Get all messages where the user is either sender or receiver
            var userMessages = await _context.ChatMessages
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .Where(m => m.SenderId == userId || m.ReceiverId == userId)
                .ToListAsync();

            // Group by the "other" user ID
            var conversations = userMessages
                .GroupBy(m => m.SenderId == userId ? m.ReceiverId : m.SenderId)
                .Select(group =>
                {
                    var otherUserId = group.Key;
                    var lastMessage = group.OrderByDescending(m => m.SentAt).First();
                    // Grab the other user's model to get their name
                    var otherUser = lastMessage.SenderId == otherUserId ? lastMessage.Sender : lastMessage.Receiver;

                    var unreadCount = group.Count(m => m.ReceiverId == userId && !m.IsRead);

                    return new ConversationSummaryDto
                    {
                        OtherUserId = otherUserId,
                        OtherUserName = otherUser != null ? $"{otherUser.FirstName} {otherUser.LastName}" : "Unknown User",
                        OtherUserRole = "", // You might need to query the userManager for roles separately if strictly needed
                        LastMessageText = lastMessage.MessageText ?? (lastMessage.MessageType == "image" ? "📷 Image" : ""),
                        LastMessageType = lastMessage.MessageType,
                        LastMessageTime = lastMessage.SentAt,
                        UnreadCount = unreadCount
                    };
                })
                .OrderByDescending(c => c.LastMessageTime)
                .ToList();

            return conversations;
        }
    }
}
