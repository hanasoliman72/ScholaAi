using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using ScholaAi.Hubs;
using ScholaAi.Models;
using ScholaAi.Services.Base;
using ScholaAi.Services.User;
using System.Security.Claims; // ADDED: Required for ClaimTypes

namespace ScholaAi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly IFileUploadService _fileService;
        private readonly IChatService _chatService;
        private readonly IHubContext<ChatHub> _hub;

        public UploadController(
            IFileUploadService fileService,
            IChatService chatService,
            IHubContext<ChatHub> hub)
        {
            _fileService = fileService;
            _chatService = chatService;
            _hub = hub;
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(
            IFormFile file,
            string receiverId)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

     
            var fileUrl = await _fileService.UploadFileAsync(file, "uploads");

            if (fileUrl == null)
                return BadRequest("Upload failed.");

            // FIX: Changed "sub" to ClaimTypes.NameIdentifier because that is what your Identity system uses in the token.
            var senderId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(senderId))
                return Unauthorized();


            var savedMessage = await _chatService.SaveMessageAsync(
                senderId,
                receiverId,
                messageText: null,         
                attachmentUrl: fileUrl,
                messageType: "image"       
            );

           
            await _hub.Clients.User(receiverId)
                .SendAsync("ReceiveMessage", savedMessage);

            await _hub.Clients.User(senderId)
                .SendAsync("ReceiveMessage", savedMessage);

            return Ok(new { url = fileUrl });
        }
    }
}