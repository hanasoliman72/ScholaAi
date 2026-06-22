using ScholaAi.Models;

namespace ScholaAi.Repositories.Base
{
    public interface IChatRepository
    {
        Task<ChatMessage> AddAsync(ChatMessage message);
        Task<List<ChatMessage>> GetChatHistoryAsync(string userId1, string userId2);
        Task<List<ScholaAi.DTOs.Chat.ConversationSummaryDto>> GetUserConversationsAsync(string userId);
        Task MarkMessagesAsReadAsync(string userId, string senderId);
    }
}
