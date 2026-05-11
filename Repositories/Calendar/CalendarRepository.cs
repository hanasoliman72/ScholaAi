using Microsoft.EntityFrameworkCore;
using ScholaAi.DTOs.Calendar;
using ScholaAi.Repositories.Base;
using ScholaAi.Models;

namespace ScholaAi.Repositories.Calendar
{
    public class CalendarRepository : ICalendarRepository
    {
        private readonly DBcontext _context;

        public CalendarRepository(DBcontext context)
        {
            _context = context;
        }

        // ═══════════════════════════════════════════════════════
        // STUDENT
        // ═══════════════════════════════════════════════════════
        public async Task<StudentCalendarMonthDto> GetStudentMonthAsync(
            string studentId, int year, int month)
        {
            var sessions = await _context.Sessions
                .Include(s => s.SessionRequest)
                    .ThenInclude(r => r.Subject)
                .Include(s => s.Teacher)
                    .ThenInclude(t => t.ApplicationUser)
                .Where(s =>
                    s.StudentId == studentId &&
                    s.SessionRequest.FinalScheduledAt.HasValue &&
                    s.SessionRequest.FinalScheduledAt.Value.Year == year &&
                    s.SessionRequest.FinalScheduledAt.Value.Month == month)
                .ToListAsync();

            var now = DateTime.UtcNow;
            var datesWithSessions = new Dictionary<int, string>();

            foreach (var s in sessions)
            {
                var day = s.SessionRequest.FinalScheduledAt!.Value.Day;
                var status = GetSessionStatus(s, now);
                var color = GetStatusColor(status);

                // Only store one color per day (priority: upcoming > pending > completed)
                if (!datesWithSessions.ContainsKey(day))
                    datesWithSessions[day] = color;
                else if (status == "Upcoming")
                    datesWithSessions[day] = color;
            }

            return new StudentCalendarMonthDto
            {
                Year = year,
                Month = month,
                TotalSessions = sessions.Count,
                CompletedSessions = sessions.Count(s => s.FocusScore.HasValue),
                UpcomingSessions = sessions.Count(s =>
                    s.SessionRequest.Status == RequestStatus.Accepted &&
                    s.SessionRequest.FinalScheduledAt.HasValue &&
                    s.SessionRequest.FinalScheduledAt.Value > now &&
                    !s.FocusScore.HasValue),
                DatesWithSessions = datesWithSessions
            };
        }

        public async Task<StudentCalendarDayDetailDto> GetStudentDayAsync(
            string studentId, DateTime date)
        {
            var now = DateTime.UtcNow;

            // Get sessions for the specific day
            var daySessions = await _context.Sessions
                .Include(s => s.SessionRequest)
                    .ThenInclude(r => r.Subject)
                .Include(s => s.Teacher)
                    .ThenInclude(t => t.ApplicationUser)
                .Where(s =>
                    s.StudentId == studentId &&
                    s.SessionRequest.FinalScheduledAt.HasValue &&
                    s.SessionRequest.FinalScheduledAt.Value.Date == date.Date)
                .ToListAsync();

            // Get month summary
            var monthSessions = await _context.Sessions
                .Include(s => s.SessionRequest)
                .Where(s =>
                    s.StudentId == studentId &&
                    s.SessionRequest.FinalScheduledAt.HasValue &&
                    s.SessionRequest.FinalScheduledAt.Value.Year == date.Year &&
                    s.SessionRequest.FinalScheduledAt.Value.Month == date.Month)
                .ToListAsync();

            var sessionDtos = daySessions.Select(s =>
            {
                var scheduledAt = s.SessionRequest.FinalScheduledAt!.Value;
                var status = GetSessionStatus(s, now);
                var hours = s.RecordingDuration > 0
                                  ? s.RecordingDuration / 3600.0
                                  : 1.0;

                return new StudentCalendarSessionDto
                {
                    SessionId = s.SessionId,
                    SubjectName = s.SessionRequest?.Subject?.name ?? "N/A",
                    TeacherName = s.Teacher?.ApplicationUser?.FirstName + " "
                                + s.Teacher?.ApplicationUser?.LastName,
                    ScheduledAt = scheduledAt,
                    Time = scheduledAt.ToString("h:mm tt"),
                    Duration = FormatDuration(hours),
                    Status = status,
                    FocusScore = s.FocusScore,
                    HasNotes = !string.IsNullOrEmpty(s.Summary)
                };
            }).ToList();

            return new StudentCalendarDayDetailDto
            {
                Date = date,
                Sessions = sessionDtos,
                TotalSessionsThisMonth = monthSessions.Count,
                CompletedThisMonth = monthSessions.Count(s => s.FocusScore.HasValue),
                UpcomingThisMonth = monthSessions.Count(s =>
                    s.SessionRequest.Status == RequestStatus.Accepted &&
                    s.SessionRequest.FinalScheduledAt.HasValue &&
                    s.SessionRequest.FinalScheduledAt.Value > now &&
                    !s.FocusScore.HasValue)
                
            };
        }
        public async Task<StudentSessionNotesDto?> GetSessionNotesAsync(
            string studentId, int sessionId)
        {
            var session = await _context.Sessions
                .Include(s => s.SessionRequest)
                    .ThenInclude(r => r.Subject)
                .Include(s => s.Teacher)
                    .ThenInclude(t => t.ApplicationUser)
                .FirstOrDefaultAsync(s =>
                    s.SessionId == sessionId &&
                    s.StudentId == studentId);

            if (session == null) return null;

            return new StudentSessionNotesDto
            {
                SessionId = session.SessionId,
                Summary = session.Summary,
                FocusScore = session.FocusScore,
                SubjectName = session.SessionRequest?.Subject?.name ?? "N/A",
                TeacherName = session.Teacher?.ApplicationUser?.FirstName + " "
                            + session.Teacher?.ApplicationUser?.LastName,
                ScheduledAt = session.SessionRequest?.FinalScheduledAt ?? DateTime.UtcNow
            };
        }

        // ═══════════════════════════════════════════════════════
        // TEACHER
        // ═══════════════════════════════════════════════════════
        public async Task<TeacherCalendarMonthDto> GetTeacherMonthAsync(
            string teacherId, int year, int month)
        {
            var sessions = await _context.Sessions
                .Include(s => s.SessionRequest)
                .Where(s =>
                    s.TeacherId == teacherId &&
                    s.SessionRequest.FinalScheduledAt.HasValue &&
                    s.SessionRequest.FinalScheduledAt.Value.Year == year &&
                    s.SessionRequest.FinalScheduledAt.Value.Month == month)
                .ToListAsync();

            var now = DateTime.UtcNow;
            var datesWithSessions = new Dictionary<int, string>();

            foreach (var s in sessions)
            {
                var day = s.SessionRequest.FinalScheduledAt!.Value.Day;
                var status = GetSessionStatus(s, now);
                var color = GetStatusColor(status);

                if (!datesWithSessions.ContainsKey(day))
                    datesWithSessions[day] = color;
                else if (status == "Upcoming")
                    datesWithSessions[day] = color;
            }

            return new TeacherCalendarMonthDto
            {
                Year = year,
                Month = month,
                TotalSessions = sessions.Count,
                CompletedSessions = sessions.Count(s => s.FocusScore.HasValue),
                UpcomingSessions = sessions.Count(s =>
                    s.SessionRequest.Status == RequestStatus.Accepted &&
                    s.SessionRequest.FinalScheduledAt.HasValue &&
                    s.SessionRequest.FinalScheduledAt.Value > now &&
                   !s.FocusScore.HasValue),
                DatesWithSessions = datesWithSessions
            };
        }

        public async Task<TeacherCalendarDayDetailDto> GetTeacherDayAsync(
            string teacherId, DateTime date)
        {
            var now = DateTime.UtcNow;

            var daySessions = await _context.Sessions
                .Include(s => s.SessionRequest)
                    .ThenInclude(r => r.Subject)
                .Include(s => s.Student)
                    .ThenInclude(st => st.ApplicationUser)
                .Where(s =>
                    s.TeacherId == teacherId &&
                    s.SessionRequest.FinalScheduledAt.HasValue &&
                    s.SessionRequest.FinalScheduledAt.Value.Date == date.Date)
                .ToListAsync();

            var monthSessions = await _context.Sessions
                .Include(s => s.SessionRequest)
                .Where(s =>
                    s.TeacherId == teacherId &&
                    s.SessionRequest.FinalScheduledAt.HasValue &&
                    s.SessionRequest.FinalScheduledAt.Value.Year == date.Year &&
                    s.SessionRequest.FinalScheduledAt.Value.Month == date.Month)
                .ToListAsync();

            var sessionDtos = daySessions.Select(s =>
            {
                var scheduledAt = s.SessionRequest.FinalScheduledAt!.Value;
                var status = GetSessionStatus(s, now);
                var hours = s.RecordingDuration > 0
                                  ? s.RecordingDuration / 3600.0
                                  : 1.0;

                return new TeacherCalendarSessionDto
                {
                    SessionId = s.SessionId,
                    SubjectName = s.SessionRequest?.Subject?.name ?? "N/A",
                    StudentName = s.Student?.ApplicationUser?.FirstName + " "
                                + s.Student?.ApplicationUser?.LastName,
                    ScheduledAt = scheduledAt,
                    Time = scheduledAt.ToString("h:mm tt"),
                    Duration = FormatDuration(hours),
                    Status = status,
                    FocusScore = s.FocusScore
                };
            }).ToList();

            return new TeacherCalendarDayDetailDto
            {
                Date = date,
                Sessions = sessionDtos,
                TotalSessionsThisMonth = monthSessions.Count,
                CompletedThisMonth = monthSessions.Count(s => s.FocusScore.HasValue),
                UpcomingThisMonth = monthSessions.Count(s =>
                    s.SessionRequest.Status == RequestStatus.Accepted &&
                    s.SessionRequest.FinalScheduledAt.HasValue &&
                    s.SessionRequest.FinalScheduledAt.Value > now &&
                   !s.FocusScore.HasValue)

            };
        }

        public async Task<TeacherSessionAnalysisDto?> GetSessionAnalysisAsync(
            string teacherId, int sessionId)
        {
            var session = await _context.Sessions
                .Include(s => s.SessionRequest)
                    .ThenInclude(r => r.Subject)
                .Include(s => s.Student)
                    .ThenInclude(st => st.ApplicationUser)
                .FirstOrDefaultAsync(s =>
                    s.SessionId == sessionId &&
                    s.TeacherId == teacherId);

            if (session == null) return null;

            var hours = session.RecordingDuration > 0
                        ? session.RecordingDuration / 3600.0
                        : 1.0;

            return new TeacherSessionAnalysisDto
            {
                SessionId = session.SessionId,
                StudentName = session.Student?.ApplicationUser?.FirstName + " "
                                       + session.Student?.ApplicationUser?.LastName,
                SubjectName = session.SessionRequest?.Subject?.name ?? "N/A",
                ScheduledAt = session.SessionRequest?.FinalScheduledAt ?? DateTime.UtcNow,
                FocusScore = session.FocusScore,
                Summary = session.Summary,
                RecordedSessionSeconds = session.RecordingDuration,
                Duration = hours == 1.0 ? "1 hour" :
                                         hours < 1.0 ? $"{hours * 60:0} minutes" :
                                         $"{hours:0.#} hours"
            };
        }

        // ═══════════════════════════════════════════════════════
        // PRIVATE HELPERS
        // ═══════════════════════════════════════════════════════

        private static string GetSessionStatus(Session s, DateTime now)
        {
            // Completed = has a focus score
            if (s.FocusScore.HasValue)
                return "Completed";

            // Pending = request not yet accepted
            if (s.SessionRequest?.Status != RequestStatus.Accepted)
                return "Pending";

            // Upcoming = accepted and scheduled in the future
            if (s.SessionRequest?.FinalScheduledAt.HasValue == true &&
                s.SessionRequest.FinalScheduledAt.Value > now)
                return "Upcoming";

            // Default to Pending
            return "Pending";
        }
        //private static string GetSessionStatus(Session s, DateTime now)
        //{
        //    if (s.FocusScore.HasValue)
        //        return "Completed";

        //    if (s.SessionRequest?.FinalScheduledAt.HasValue == true)
        //    {
        //        if (s.SessionRequest.FinalScheduledAt.Value > now)
        //            return "Upcoming";
        //    }

        //    return "Pending";
        //}


        private static string GetStatusColor(string status)
        {
            return status switch
            {
                "Upcoming" => "blue",
                "Completed" => "green",
                "Pending" => "yellow",
                _ => "grey"
            };
        }

        private static string FormatDuration(double hours)
        {
            if (hours == 1.0) return "1 hour";
            if (hours < 1.0) return $"{hours * 60:0} minutes";
            return $"{hours:0.#} hours";
        }
    }
}