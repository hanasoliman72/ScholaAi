using ScholaAi.Models;

namespace ScholaAi.Repositories.Base
{
    public interface IChatRepository
    {
        Task<ChatMessage> AddAsync(ChatMessage message);
    }
}
