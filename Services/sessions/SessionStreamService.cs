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
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;
        private readonly IServiceScopeFactory _scopeFactory;

        public SessionStreamService(
            ISessionRepository sessionRepo,
            ISessionRequestRepository requestRepo,
            IConfiguration config,
            HttpClient httpClient,
            IServiceScopeFactory scopeFactory)
        {
            _sessionRepo = sessionRepo;
            _requestRepo = requestRepo;
            _config = config;
            _httpClient = httpClient;
            _scopeFactory = scopeFactory;
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
                RecordedSession = session.RecordedSession,  
                Summary = session.Summary,           
                FocusScore = session.FocusScore,        
                RecordingDuration = session.RecordingDuration, 
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

        public async Task SaveRecording(
        string teacherId, int sessionId,
        string recordingUrl, int duration)
        {
            var session = await _sessionRepo.GetByIdAsync(sessionId)
                ?? throw new Exception("Session not found");

            if (session.TeacherId != teacherId)
                throw new Exception("Not authorized");

            session.RecordedSession = recordingUrl;
            session.RecordingDuration = duration;

            await _sessionRepo.SaveAsync();

            // IF YOU KNOW YOU KNOW
            _ = Task.Run(() => GenerateSummaryAsync(sessionId, recordingUrl)); 
        }

        public async Task GenerateSummaryAsync(int sessionId, string videoUrl)
        {
            try
            {
                var summaryUrl = _config["PythonAI:SummaryUrl"];

                var response = await _httpClient.PostAsJsonAsync(summaryUrl, new
                {
                    video_url = videoUrl,
                    session_id = sessionId,
                });

                if (!response.IsSuccessStatusCode) return;

                var result = await response.Content
                    .ReadFromJsonAsync<SummaryResponse>();

                if (result?.Success != true || string.IsNullOrEmpty(result.Summary))
                    return;

                // create a fresh scope so DbContext is not disposed
                // this sessionRepo has a FRESH DbContext that belongs to this background task
                // it won't be destroyed until we call scope.Dispose() (which `using` does automatically)
                using var scope = _scopeFactory.CreateScope();
                var sessionRepo = scope.ServiceProvider
                    .GetRequiredService<ISessionRepository>();

                // save summary to DB
                var session = await sessionRepo.GetByIdAsync(sessionId);
                if (session == null) return;

                session.Summary = result.Summary;
                await sessionRepo.SaveAsync();
                Console.WriteLine($"[Summary] ✅ Saved for session {sessionId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Summary] ❌ Failed: {ex.Message}");
            }
        }
    }

    public class SummaryResponse
    {
        public bool Success { get; set; }
        public string? Summary { get; set; }
        public string? Transcript { get; set; }
    }
}