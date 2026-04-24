using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScholaAi.Services.Base;
using System.Security.Claims;

namespace ScholaAi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpGet("conversations")]
        public async Task<IActionResult> GetConversations()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var conversations = await _chatService.GetUserConversationsAsync(userId);
            return Ok(conversations);
        }

        [HttpGet("history/{otherUserId}")]
        public async Task<IActionResult> GetChatHistory(string otherUserId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var history = await _chatService.GetChatHistoryAsync(userId, otherUserId);
            
            // Map the raw ChatMessage models to a cleaner DTO structure if needed,
            // but for now we return the whole object to match the SignalR payload.
            var response = history.Select(m => new
            {
                messageId = m.MessageId,
                senderId = m.SenderId,
                receiverId = m.ReceiverId,
                messageText = m.MessageText,
                attachmentURL = m.AttachmentURL,
                messageType = m.MessageType,
                isRead = m.IsRead,
                sentAt = m.SentAt
            });

            return Ok(response);
        }
    }
}
