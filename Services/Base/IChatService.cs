using ScholaAi.Models;

namespace ScholaAi.Services.Base
{
    public interface IChatService
    {
        Task<ChatMessage> SaveMessageAsync(
       string senderId,
       string receiverId,
       string? messageText,
       string? attachmentUrl,
       string messageType);

        Task<List<ChatMessage>> GetChatHistoryAsync(string userId1, string userId2);
        Task<List<ScholaAi.DTOs.Chat.ConversationSummaryDto>> GetUserConversationsAsync(string userId);
        Task MarkMessagesAsReadAsync(string userId, string senderId);
    }
}
