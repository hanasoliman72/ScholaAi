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
    }
}

