using Microsoft.AspNetCore.SignalR;
using ScholaAi.DTOs.Sessions;
using ScholaAi.Hubs;
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
        private readonly IHubContext<SessionHub> _sessionHub;
        private readonly IWalletService _walletService;

        public SessionStreamService(
            ISessionRepository sessionRepo,
            ISessionRequestRepository requestRepo,
            IConfiguration config,
            HttpClient httpClient,
            IServiceScopeFactory scopeFactory,
            IHubContext<SessionHub> sessionHub,
            IWalletService walletService)
        {
            _sessionRepo = sessionRepo;
            _requestRepo = requestRepo;
            _config = config;
            _httpClient = httpClient;
            _scopeFactory = scopeFactory;
            _sessionHub = sessionHub;
            _walletService = walletService;
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
                Subject = session.SessionRequest?.Subject?.name,
                LessonTitle = session.SessionRequest?.Description,
            };
        }

        public async Task<List<studentSessionDto>> GetStudentSessions(string studentId)
        {
            var sessions = await _sessionRepo.GetByStudentIdAsync(studentId);
            if (sessions == null) return new List<studentSessionDto>();

            var dtos = new List<studentSessionDto>();
            foreach (var session in sessions)
            {
                var teacherFirstName = session.Teacher?.ApplicationUser?.FirstName ?? "";
                var teacherLastName = session.Teacher?.ApplicationUser?.LastName ?? "";
                var teacherFullName = $"{teacherFirstName} {teacherLastName}".Trim();
                if (string.IsNullOrEmpty(teacherFullName)) teacherFullName = "Teacher";

                var initials = "";
                if (!string.IsNullOrEmpty(teacherFirstName)) initials += teacherFirstName[0];
                if (!string.IsNullOrEmpty(teacherLastName)) initials += teacherLastName[0];
                if (string.IsNullOrEmpty(initials)) initials = "T";

                var sessionDate = session.StartedAt?.ToString("MMM d, yyyy") 
                    ?? session.SessionRequest?.PreferredDate.ToString("MMM d, yyyy") 
                    ?? DateTime.UtcNow.ToString("MMM d, yyyy");

                var durationStr = "0m";
                if (session.RecordingDuration > 0)
                {
                    var ts = TimeSpan.FromSeconds(session.RecordingDuration);
                    durationStr = ts.Hours > 0 ? $"{ts.Hours}h {ts.Minutes}m" : $"{ts.Minutes}m";
                }

                dtos.Add(new studentSessionDto
                {
                    id = session.SessionId,
                    subject = session.SessionRequest?.Subject?.name ?? "Other",
                    lessonTitle = session.SessionRequest?.Description ?? "Private Session",
                    teacher = teacherFullName,
                    teacherInitials = initials.ToUpper(),
                    date = sessionDate,
                    duration = durationStr,
                    focusScore = session.FocusScore,
                    status = session.Status,
                    recordedSession = session.RecordedSession,
                    summary = session.Summary,
                    ratingId = session.Rating?.RatingId,
                    ratingValue = session.Rating?.RatingValue
                });
            }

            return dtos;
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

            // prevent starting if teacher already has an active session
            if (await _sessionRepo.HasActiveSessionForTeacherAsync(teacherId))
                throw new Exception("You already have an active session. Please end it before starting a new one.");

            // prevent starting if student already has an active session
            if (await _sessionRepo.HasActiveSessionForStudentAsync(request.StudentId))
                throw new Exception("The student is already in another active session.");

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

            // Calculate duration in minutes (1 minute = 1 $)
            int minutes = 0;
            if (session.StartedAt.HasValue)
            {
                var duration = session.EndedAt.Value - session.StartedAt.Value;
                minutes = (int)Math.Ceiling(duration.TotalMinutes);
            }
            if (minutes < 1) 
            {
                minutes = 1; // Default to minimum of 1 minute charge if the session was active
            }
            decimal amount = minutes;

            // Transfer amount from student to teacher wallet
            await _walletService.DebitWalletAsync(session.StudentId, amount);
            await _walletService.CreditWalletAsync(session.TeacherId, amount);
            await _walletService.RecordTransactionAsync(session.StudentId, session.TeacherId, session.SessionId, amount, 0);

            await _sessionRepo.SaveAsync();
        }

        /// <summary>
        /// Called periodically by focus_server.py (student's machine) to update
        /// the live FocusScore in the DB during an active session.
        /// </summary>
        public async Task ReportFocusAsync(string studentId, int sessionId, int focusScore)
        {
            var session = await _sessionRepo.GetByIdAsync(sessionId)
                ?? throw new Exception("Session not found");

            if (session.StudentId != studentId)
                throw new Exception("Not authorized");

            if (session.Status != "active")
                throw new Exception("Session is not active");

            session.FocusScore = Math.Clamp(focusScore, 0, 100);
            await _sessionRepo.SaveAsync();
        }

        /// <summary>
        /// Called by focus_server.py when distraction is detected.
        /// Fires a SignalR DistractionAlert event directly to the teacher (host)
        /// in the session room — no client-side SignalR dependency needed in Python.
        /// </summary>
        public async Task NotifyDistractionAsync(string studentId, int sessionId, string roomId, string reason)
        {
            var session = await _sessionRepo.GetByIdAsync(sessionId)
                ?? throw new Exception("Session not found");

            if (session.StudentId != studentId)
                throw new Exception("Not authorized");

            // Broadcast DistractionAlert to every connection in the room group.
            // SessionHub.StudentDistracted targets host connections only — we use the
            // group shortcut here so the hub's room filtering isn't duplicated.
            await _sessionHub.Clients.Group(roomId).SendAsync("DistractionAlert", reason);
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