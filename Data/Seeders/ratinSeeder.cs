using ScholaAi.Models;

namespace ScholaAi.Data.Seeders
{
    public class ratingSeeder
    {
        public static void SeedRatingData(DBcontext context)
        {
            // =========================
            // 1. SESSION REQUESTS
            // =========================
            var sessionRequest1 = new sessionRequest
            {
                studentId = 6,
                teacherId = 10,
                subjectId = 2,
                preferredDate = DateTime.Now.AddDays(-5),
                status = requestStatus.Accepted,
                description = "Math tutoring session",
                finalScheduledAt = DateTime.Now.AddDays(-5),
                createdAt = DateTime.Now.AddDays(-10)
            };

            context.sessionRequests.AddRange(
                sessionRequest1
            );
            context.SaveChanges();

            // Load saved session requests
            var sessionRequests = context.sessionRequests
                .OrderBy(sr => sr.createdAt)
                .Take(1)
                .ToList();

            //Console.WriteLine(sessionRequests.Count);
            // =========================
            // 2. SESSIONS
            // =========================
            var session1 = new session
            {
                requestId = sessionRequests[0].sessionId,
                teacherId = 10,
                studentId = sessionRequests[0].studentId,
                recordedSession = 3600,
                summary = "Covered algebra basics, student understood well",
                focusScore = 85
            };

            context.sessions.AddRange(session1);
            context.SaveChanges();

            // Load saved sessions
            //var sessions = context.sessions
            //    .OrderBy(s => s.sessionId)
            //    .Take(3)
            //    .ToList();

            //// =========================
            //// 3. RATINGS
            //// =========================
            //if (!context.ratings.Any())
            //{
            //    var rating1 = new rating
            //    {
            //        sessionId = sessions[0].sessionId,
            //        studentId = sessions[0].studentId,
            //        teacherId = sessions[0].teacherId,
            //        ratingValue = 5,
            //        comment = "Excellent teacher! Very patient and explains well.",
            //        createdAt = DateTime.Now.AddDays(-4)
            //    };

            //    var rating2 = new rating
            //    {
            //        sessionId = sessions[1].sessionId,
            //        studentId = sessions[1].studentId,
            //        teacherId = sessions[1].teacherId,
            //        ratingValue = 4,
            //        comment = "Good session, learned a lot about grammar.",
            //        createdAt = DateTime.Now.AddDays(-2)
            //    };
            //    context.ratings.AddRange(rating1, rating2);
            //    context.SaveChanges();
            //}
        }
    }
}
