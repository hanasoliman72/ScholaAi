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

    }
}
