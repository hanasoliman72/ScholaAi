using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using ScholaAi.Models;
using ScholaAi.Services.Base;

namespace ScholaAi.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IChatService _chatService;

        public ChatHub(IChatService chatService)
        {
            _chatService = chatService;
        }

        
        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;
            Console.WriteLine($"User Connected: {userId}");
            await base.OnConnectedAsync();
        }
        public async Task SendPrivateMessage(
            string receiverId,
            string messageText)
        {
            var senderId = Context.UserIdentifier;

            if (string.IsNullOrEmpty(senderId))
                return;

            if (string.IsNullOrWhiteSpace(messageText))
                return;

            var savedMessage = await _chatService
                .SaveMessageAsync(
                    senderId,
                    receiverId,
                    messageText,
                    null,
                    "text");

            await Clients.User(receiverId)
                .SendAsync("ReceiveMessage", savedMessage);

            await Clients.User(senderId)
                .SendAsync("ReceiveMessage", savedMessage);
        }
    }
}
