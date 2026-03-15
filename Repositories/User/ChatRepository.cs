using Microsoft.AspNetCore.Identity;
using ScholaAi.Models;
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
    }
}
