using ScholaAi.Models;
using ScholaAi.Repositories.Base;
using ScholaAi.Services.Base;

namespace ScholaAi.Services.User
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _repo;

        public ChatService(IChatRepository repo)
        {
            _repo = repo;
        }

        public async Task<ChatMessage> SaveMessageAsync(
            string senderId,
            string receiverId,
            string? messageText,
            string? attachmentUrl,
            string messageType)
        {
            var message = new ChatMessage
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                MessageText = messageText,
                AttachmentURL = attachmentUrl,
                // FIX: Actually save the messageType argument to the database so it's not discarded.
                MessageType = messageType,
                SentAt = DateTime.UtcNow
            };

            return await _repo.AddAsync(message);
        }

        public async Task<List<ChatMessage>> GetChatHistoryAsync(string userId1, string userId2)
        {
            return await _repo.GetChatHistoryAsync(userId1, userId2);
        }

        public async Task<List<ScholaAi.DTOs.Chat.ConversationSummaryDto>> GetUserConversationsAsync(string userId)
        {
            return await _repo.GetUserConversationsAsync(userId);
        }

        public async Task MarkMessagesAsReadAsync(string userId, string senderId)
        {
            await _repo.MarkMessagesAsReadAsync(userId, senderId);
        }
    }
}

