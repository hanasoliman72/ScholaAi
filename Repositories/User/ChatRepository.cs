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
            // Get the IDs of the latest messages in each conversation (group by the other user)
            var latestMessageIds = await _context.ChatMessages
                .Where(m => m.SenderId == userId || m.ReceiverId == userId)
                .GroupBy(m => m.SenderId == userId ? m.ReceiverId : m.SenderId)
                .Select(g => g.Max(m => m.MessageId))
                .ToListAsync();

            // Load those messages with their related user profiles, and project to DTO
            var conversations = await _context.ChatMessages
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .Where(m => latestMessageIds.Contains(m.MessageId))
                .Select(m => new ConversationSummaryDto
                {
                    OtherUserId = m.SenderId == userId ? m.ReceiverId : m.SenderId,
                    OtherUserName = m.SenderId == userId 
                        ? (m.Receiver != null ? m.Receiver.FirstName + " " + m.Receiver.LastName : "Unknown User")
                        : (m.Sender != null ? m.Sender.FirstName + " " + m.Sender.LastName : "Unknown User"),
                    OtherUserRole = "",
                    LastMessageText = m.MessageText ?? (m.MessageType == "image" ? "📷 Image" : ""),
                    LastMessageType = m.MessageType,
                    LastMessageTime = m.SentAt,
                    UnreadCount = _context.ChatMessages.Count(x => 
                        x.SenderId == (m.SenderId == userId ? m.ReceiverId : m.SenderId) && 
                        x.ReceiverId == userId && 
                        !x.IsRead)
                })
                .OrderByDescending(c => c.LastMessageTime)
                .ToListAsync();

            return conversations;
        }

        public async Task MarkMessagesAsReadAsync(string userId, string senderId)
        {
            var unreadMessages = await _context.ChatMessages
                .Where(m => m.ReceiverId == userId && m.SenderId == senderId && !m.IsRead)
                .ToListAsync();

            if (unreadMessages.Any())
            {
                foreach (var msg in unreadMessages)
                {
                    msg.IsRead = true;
                }
                await _context.SaveChangesAsync();
            }
        }
    }
}
