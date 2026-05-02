using Microsoft.EntityFrameworkCore;
using ScholaAi.DTOs.Sessions;
using ScholaAi.Models;
using ScholaAi.Repositories.Base;
using ScholaAi.Services.Base;

namespace ScholaAi.Services.sessions
{
    public class SessionStreamService : ISessionStreamService
    {
        //private readonly ISessionRequestRepository _requestRepo;
        //private readonly IRequestBroadcastRepository _broadcastRepo;
        //private readonly INotificationService _notificationService;
        private readonly DBcontext _context;

        public SessionStreamService(DBcontext context)
        {
           _context = context;

        }

        public async Task<Session> GetSessionById(int sessionId)
        {
            return await _context.Sessions
                .Include(s => s.Teacher)
                .Include(s => s.Student)
                .FirstOrDefaultAsync(s => s.SessionId == sessionId)
                ?? throw new Exception("Session not found");
        }

        public async Task<StartSessionResponseDto> StartSession(string teacherId, int sessionId)
        {
            var session = await _context.Sessions
                .FirstOrDefaultAsync(s => s.SessionId == sessionId && s.TeacherId == teacherId)
                ?? throw new Exception("Session not found or unauthorized");

            if (session.Status == "active")
                throw new Exception("Session already started");

            // Generate a unique room ID for mediasoup
            session.RoomId = $"room-{sessionId}-{Guid.NewGuid().ToString("N")[..8]}";
            session.Status = "active";
            session.StartedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new StartSessionResponseDto
            {
                RoomId = session.RoomId,
                PeerId = teacherId,
                Role = "host",
                SessionId = sessionId,
            };
        }

        public async Task<StartSessionResponseDto> JoinSession(string studentId, int sessionId)
        {
            var session = await _context.Sessions
                .FirstOrDefaultAsync(s => s.SessionId == sessionId && s.StudentId == studentId)
                ?? throw new Exception("Session not found or unauthorized");

            if (session.Status != "active")
                throw new Exception("Session is not active yet. Wait for the teacher to start.");

            return new StartSessionResponseDto
            {
                RoomId = session.RoomId,
                PeerId = studentId,
                Role = "viewer",
                SessionId = sessionId,
            };
        }

        public async Task EndSession(string teacherId, int sessionId)
        {
            var session = await _context.Sessions
                .FirstOrDefaultAsync(s => s.SessionId == sessionId && s.TeacherId == teacherId)
                ?? throw new Exception("Session not found or unauthorized");

            session.Status = "ended";
            session.EndedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
    }
}
