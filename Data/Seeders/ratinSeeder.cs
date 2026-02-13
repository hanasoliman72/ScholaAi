using ScholaAi.Models;

namespace ScholaAi.Data.Seeders
{
    /// <summary>
    /// Simple seed helper for demo data around sessions / ratings.
    /// NOTE: This assumes string-based Identity keys and the new
    /// PascalCase properties on the models.
    /// </summary>
    public class ratingSeeder
    {
        public static void SeedRatingData(DBcontext context)
        {
            // =========================
            // 1. SESSION REQUESTS
            // =========================
            // These IDs are sample string user IDs; adjust to real ones
            // if you wire this seeder into your startup.
            var sessionRequest1 = new SessionRequest
            {
                StudentId = "3",
                TeacherId = "2",
                SubjectId = 1,
                PreferredDate = DateTime.UtcNow.AddDays(-5),
                Status = RequestStatus.Accepted,
                Description = "c++ tutoring Session",
                FinalScheduledAt = DateTime.UtcNow.AddDays(-5),
                CreatedAt = DateTime.UtcNow.AddDays(-10)
            };

            var sessionRequest2 = new SessionRequest
            {
                StudentId = "3",
                TeacherId = "2",
                SubjectId = 2,
                PreferredDate = DateTime.UtcNow.AddDays(-5),
                Status = RequestStatus.Accepted,
                Description = "Math tutoring Session",
                FinalScheduledAt = DateTime.UtcNow.AddDays(-5),
                CreatedAt = DateTime.UtcNow.AddDays(-10)
            };

            var sessionRequest3 = new SessionRequest
            {
                StudentId = "4",
                TeacherId = "2",
                SubjectId = 1,
                PreferredDate = DateTime.UtcNow.AddDays(-5),
                Status = RequestStatus.Accepted,
                Description = "C++ tutoring Session",
                FinalScheduledAt = DateTime.UtcNow.AddDays(-5),
                CreatedAt = DateTime.UtcNow.AddDays(-10)
            };

            context.SessionRequests.AddRange(
                sessionRequest1, sessionRequest2, sessionRequest3
            );
            context.SaveChanges();

            // Load saved Session requests
            var sessionRequests = context.SessionRequests
                .OrderBy(sr => sr.CreatedAt)
                .Take(3)
                .ToList();

            // =========================
            // 2. SESSIONS
            // =========================
            var session1 = new Session
            {
                RequestId = sessionRequests[0].RequestId,
                TeacherId = "2",
                StudentId = sessionRequests[0].StudentId,
                RecordedSession = 3600,
                Summary = "Covered algebra basics, Student understood well",
                FocusScore = 85
            };

            var session2 = new Session
            {
                RequestId = sessionRequests[1].RequestId,
                TeacherId = "2",
                StudentId = sessionRequests[1].StudentId,
                RecordedSession = 3600,
                Summary = "Covered algebra basics, Student understood well",
                FocusScore = 85
            };

            var session3 = new Session
            {
                RequestId = sessionRequests[2].RequestId,
                TeacherId = "2",
                StudentId = sessionRequests[2].StudentId,
                RecordedSession = 3600,
                Summary = "Covered algebra basics, Student understood well",
                FocusScore = 85
            };

            context.Sessions.AddRange(session1, session2, session3);
            context.SaveChanges();
        }
    }
}
