using ScholaAi.DTOs.Sessions;
using ScholaAi.Models;
using ScholaAi.Repositories.Base;
using ScholaAi.Repositories.sessions;
using ScholaAi.Services.Base;

namespace ScholaAi.Services.sessions
{
    public class SessionStreamService : ISessionStreamService
    {
        private readonly ISessionRepository _sessionRepo;
        private readonly ISessionRequestRepository _requestRepo;

        public SessionStreamService(
            ISessionRepository sessionRepo,
            ISessionRequestRepository requestRepo)
        {
            _sessionRepo = sessionRepo;
            _requestRepo = requestRepo;
        }

        public async Task<SessionDetailsDto> GetSessionById(int sessionId)
        {
            var session = await _sessionRepo.GetByIdAsync(sessionId)
                ?? throw new Exception("Session not found");

            return new SessionDetailsDto
            {
                SessionId = session.SessionId,
                TeacherId = session.TeacherId,
                StudentId = session.StudentId,
                Status = session.Status,
                RoomId = session.RoomId,
                StartedAt = session.StartedAt,
                EndedAt = session.EndedAt,
                TeacherName = $"{session.Teacher.ApplicationUser.FirstName} {session.Teacher.ApplicationUser.LastName}",
                StudentName = $"{session.Student.ApplicationUser.FirstName} {session.Student.ApplicationUser.LastName}",
            };
        }

        public async Task<StartSessionResponseDto> StartSession(string teacherId, int requestId)
        {
            // only accepted requests can be started
            var request = await _requestRepo.GetById(requestId)
                ?? throw new Exception("Request not found");

            if (request.TeacherId != teacherId)
                throw new Exception("Not authorized");

            if (request.Status != RequestStatus.Accepted)
                throw new Exception("Request is not accepted yet");

            // prevent starting an already active session
            var existing = await _sessionRepo.GetByRequestIdAsync(requestId);

            if (existing != null && existing.Status == "active")
                throw new Exception("Session already started");

            // create or reactivate session
            var session = existing ?? new Session
            {
                RequestId = requestId,
                TeacherId = teacherId,
                StudentId = request.StudentId,
                RecordedSession = string.Empty,
                Summary = string.Empty,
                FocusScore = 0,
            };

            session.RoomId = $"room-{requestId}-{Guid.NewGuid().ToString("N")[..8]}";
            session.Status = "active";
            session.StartedAt = DateTime.UtcNow;

            if (existing == null)
                await _sessionRepo.AddAsync(session);

            await _sessionRepo.SaveAsync();

            return new StartSessionResponseDto
            {
                RoomId = session.RoomId,
                PeerId = teacherId,
                Role = "host",
                SessionId = session.SessionId,
            };
        }

        public async Task<StartSessionResponseDto> JoinSession(string studentId, int requestId)
        {
            var existing = await _sessionRepo.GetByRequestIdAsync(requestId)
                ?? throw new Exception("Session not started yet");

            // only the student of this request can join
            if (existing.StudentId != studentId)
                throw new Exception("Not authorized");

            // can only join active sessions
            if (existing.Status != "active")
                throw new Exception("Session is not active yet. Wait for the teacher to start.");

            return new StartSessionResponseDto
            {
                RoomId = existing.RoomId,
                PeerId = studentId,
                Role = "viewer",
                SessionId = existing.SessionId,
            };
        }

        public async Task EndSession(string teacherId, int sessionId, int focusScore = 0)
        {
            var session = await _sessionRepo.GetByIdAsync(sessionId)
                ?? throw new Exception("Session not found");

            // only the teacher of this session can end it
            if (session.TeacherId != teacherId)
                throw new Exception("Not authorized");

            if (session.Status == "ended")
                throw new Exception("Session already ended");

            session.Status = "ended";
            session.EndedAt = DateTime.UtcNow;
            session.FocusScore = focusScore;

            await _sessionRepo.SaveAsync();
        }

        public async Task SaveRecording(string teacherId, int sessionId, string recordingUrl, int duration)
        {
            var session = await _sessionRepo.GetByIdAsync(sessionId)
                ?? throw new Exception("Session not found");

            if (session.TeacherId != teacherId)
                throw new Exception("Not authorized");

            session.RecordedSession = recordingUrl;
            session.RecordingDuration = duration;

            await _sessionRepo.SaveAsync();
        }
    }
}