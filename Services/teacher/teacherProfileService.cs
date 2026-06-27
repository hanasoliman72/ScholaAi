using Microsoft.AspNetCore.Identity;
using ScholaAi.DTOs.Common;
using ScholaAi.DTOs.Student;
using ScholaAi.DTOs.Teacher;
using ScholaAi.DTOs.Teatcher;
using ScholaAi.Models;
using ScholaAi.Repositories.Base;
using ScholaAi.Repositories.Student;
using ScholaAi.Repositories.User;
using ScholaAi.Services.Base;

namespace ScholaAi.Services.Teacher
{
    public class teacherProfileService : ITeacherProfileService
    {
        private readonly ITeacherRepository _teacherRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFileUploadService _fileUploadService;
        private readonly IUserRepository _userRepository;
        private readonly IRatingService _ratingService;


        public teacherProfileService(
            IUserRepository userRepository,
            ITeacherRepository teacherRepository,
            UserManager<ApplicationUser> userManager,
            IFileUploadService fileUploadService,
            IRatingService ratingService)
        {
            _userRepository = userRepository;
            _teacherRepository = teacherRepository;
            _userManager = userManager;
            _fileUploadService = fileUploadService;
            _ratingService = ratingService;
        }

        // ===============================
        // Get Teacher Profile By ID (string ApplicationUserId)
        // ===============================
        public async Task<teacherProfileDto?> GetTeacherProfileAsync(string teacherId)
        {
            var teacher = await _teacherRepository.getByIdWithUserAsync(teacherId);

            if (teacher == null || teacher.ApplicationUser == null)
                return null;

            var ratingResult = await _ratingService.getTeacherAverageRatingAsync(teacherId);

            // Compute total hours taught and total sessions from teacher sessions
            var allSessions = await _teacherRepository.GetTeacherSessionsWithStudentsAsync(teacherId);
            var totalHours = Math.Round(
                allSessions
                    .Where(s => s.RecordingDuration > 0)
                    .Sum(s => s.RecordingDuration) / 3600.0m,
                2
            );

            return new teacherProfileDto
            {
                userName = teacher.ApplicationUser.UserName,
                email = teacher.ApplicationUser.Email,
                firstName = teacher.ApplicationUser.FirstName,
                lastName = teacher.ApplicationUser.LastName,
                description = teacher.ApplicationUser.Description,
                profilePhotoURL = teacher.ApplicationUser.ProfilePhotoURL,
                college = teacher.College,
                averageRate = ratingResult.averageRating,
                totalRatings = ratingResult.totalRatings,
                teachingExperience = teacher.TeachingExperience,
                totalHoursTaught = totalHours,
                totalSessions = allSessions.Count
            };
        }

        public async Task<List<teacherSearchResultDto>> SearchTeachersAsync(
               string? name,
               string? subject,
               string? keyword)
        {
            var teachers = await _teacherRepository
                .SearchTeachersAsync(name, subject, keyword);

            var list = new List<teacherSearchResultDto>();
            foreach (var t in teachers.Where(t => t.ApplicationUser != null))
            {
                var ratingResult = await _ratingService.getTeacherAverageRatingAsync(t.ApplicationUserId);
                list.Add(new teacherSearchResultDto
                {
                    userId = t.ApplicationUserId,
                    userName = t.ApplicationUser.UserName,
                    subject = t.Subject.name,
                    college = t.College,
                    teachingExperience = t.TeachingExperience,
                    profilePhotoURL = t.ApplicationUser.ProfilePhotoURL,
                    rating = (double)ratingResult.averageRating
                });
            }
            return list;
        }


        public async Task<bool> ChangePasswordAsync(string userId, changePasswordDto dto)
        {
            var user = await _userRepository.getByIdAsync(userId);
            if (user == null)
                return false;

            var identityUser = await _userManager.FindByIdAsync(user.Id);
            if (identityUser == null)
                return false;

            var result = await _userManager.ChangePasswordAsync(
                identityUser,
                dto.currentPassword,
                dto.newPassword
            );

            return result.Succeeded;
        }
        public async Task<string?> uploadProfilePhotoAsync(string userId, IFormFile file)
        {
            var user = await _userRepository.getByIdAsync(userId);
            if (user == null)
                return null;

            var photoUrl = await _fileUploadService.UploadFileAsync(file, "profile-photos");
            if (photoUrl == null)
                return null;

            user.ProfilePhotoURL = photoUrl;
            await _userRepository.updateAsync(user);

            return photoUrl;
        }
        public async Task<(bool success, string message)> UpdateTeacherProfileAsync(string userId,
            updateTeacherProfileDto dto)
        {
            var teacher = await _teacherRepository.getByIdWithUserAsync(userId);

            if (teacher == null || teacher.ApplicationUser == null)
                return (false, "Teacher profile not found.");

            var user = teacher.ApplicationUser;

            // username
            if (!string.IsNullOrWhiteSpace(dto.userName))
            {
                var userExists = await _userRepository.getUserByUserNameAsync(dto.userName);
                if (userExists != null && userExists.Id != userId)
                    return (false, "Username is already taken.");

                user.UserName = dto.userName;
            }

            // basic user info
            if (!string.IsNullOrWhiteSpace(dto.firstName))
                user.FirstName = dto.firstName;

            if (!string.IsNullOrWhiteSpace(dto.lastName))
                user.LastName = dto.lastName;

            user.PhoneNumber = string.IsNullOrWhiteSpace(dto.phone) ? null : dto.phone;
            user.Description = string.IsNullOrWhiteSpace(dto.description) ? null : dto.description;

            // teacher specific info
            if (!string.IsNullOrWhiteSpace(dto.college))
                teacher.College = dto.college;

            if (!string.IsNullOrWhiteSpace(dto.certificate))
                teacher.Certificate = dto.certificate;

            if (!string.IsNullOrWhiteSpace(dto.teachingExperience))
                teacher.TeachingExperience = dto.teachingExperience;

            await _userRepository.updateAsync(user);
            await _teacherRepository.updateAsync(teacher);

            return (true, "Teacher profile updated successfully");
        }

        // ═══════════════════════════════════════════════════════
        // MY STUDENTS
        // ═══════════════════════════════════════════════════════
        public async Task<MyStudentsListResponseDto> GetMyStudentsAsync(string teacherId, string? search)
        {
            var now = DateTime.UtcNow;
            var twoWeeksAgo = now.AddDays(-14);

            var allSessions = await _teacherRepository
                .GetTeacherSessionsWithStudentsAsync(teacherId);

            // Group by student
            var studentGroups = allSessions
                .GroupBy(s => s.StudentId)
                .ToList();

            var activeCards = new List<StudentCardDto>();
            var previousCards = new List<StudentCardDto>();

            foreach (var group in studentGroups)
            {
                var sessions = group.ToList();

                // Try multiple ways to get student info
                ApplicationUser? student = null;

                // Try through Student navigation property
                var firstSession = sessions.FirstOrDefault(s => s.Student?.ApplicationUser != null);
                if (firstSession != null)
                    student = firstSession.Student!.ApplicationUser;

                // If still null try through SessionRequest
                if (student == null)
                {
                    var sessionWithRequest = sessions.FirstOrDefault(s =>
                        s.SessionRequest?.Student?.ApplicationUser != null);
                    if (sessionWithRequest != null)
                        student = sessionWithRequest.SessionRequest!.Student!.ApplicationUser;
                }

                // If still null skip this student
                if (student == null) continue;

                var subject = sessions
                    .FirstOrDefault(s => s.SessionRequest?.Subject != null)
                    ?.SessionRequest?.Subject?.name ?? "N/A";

                var studentName = student.FirstName + " " + student.LastName;

                // Apply search filter
                if (!string.IsNullOrWhiteSpace(search))
                {
                    var searchLower = search.ToLower();
                    if (!studentName.ToLower().Contains(searchLower) &&
                        !subject.ToLower().Contains(searchLower))
                        continue;
                }

                // Stats
                var completedSessions = sessions
                    .Where(s => s.FocusScore.HasValue)
                    .ToList();

                var totalHours = Math.Round(
                sessions
                .Where(s => s.RecordingDuration > 0)
                .Sum(s => s.RecordingDuration) / 3600.0m, 2);

                double? avgFocus = completedSessions.Any()
                    ? Math.Round(completedSessions.Average(s => (double)s.FocusScore!.Value),2)
                    : null;

                var lastSession = completedSessions
                    .OrderByDescending(s => s.SessionRequest.FinalScheduledAt)
                    .FirstOrDefault();

                var nextSession = sessions
                    .Where(s =>
                        s.SessionRequest.Status == Models.RequestStatus.Accepted &&
                        s.SessionRequest.FinalScheduledAt.HasValue &&
                        s.SessionRequest.FinalScheduledAt.Value > now &&
                        !s.FocusScore.HasValue)
                    .OrderBy(s => s.SessionRequest.FinalScheduledAt)
                    .FirstOrDefault();

                var lastSessionDate = lastSession?.SessionRequest?.FinalScheduledAt;
                var hasUpcoming = nextSession != null;
                var isActive = hasUpcoming ||
                                      (lastSessionDate.HasValue &&
                                       lastSessionDate.Value >= twoWeeksAgo);

                var card = new StudentCardDto
                {
                    StudentId = group.Key,
                    StudentName = studentName,
                    ProfilePhotoURL = student.ProfilePhotoURL,
                    SubjectName = subject,
                    TotalSessions = sessions.Count,
                    TotalHours = totalHours,
                    AverageFocusScore = avgFocus,
                    LastSessionDate = lastSessionDate,
                    LastSessionAgo = GetTimeAgo(lastSessionDate),
                    NextSessionDate = nextSession?.SessionRequest?.FinalScheduledAt,
                    NextSessionTime = nextSession?.SessionRequest?.FinalScheduledAt.HasValue == true
                                        ? nextSession.SessionRequest.FinalScheduledAt.Value
                                          .ToString("h:mm tt")
                                        : null,
                    IsActive = isActive
                };

                if (isActive) activeCards.Add(card);
                else previousCards.Add(card);
            }

            var ratingResult = await _ratingService.getTeacherAverageRatingAsync(teacherId);

            var summary = new MyStudentsSummaryDto
            {
                TotalStudents = activeCards.Count + previousCards.Count,
                ActiveStudents = activeCards.Count,
                PreviousStudents = previousCards.Count,
                TotalSessions = allSessions.Count,

                TotalHoursTaught = Math.Round(
                    allSessions
                        .Where(s => s.RecordingDuration > 0)
                        .Sum(s => s.RecordingDuration) / 3600.0m,
                    2
                ),

                AverageRating = ratingResult.averageRating
            };

            return new MyStudentsListResponseDto
            {
                Summary = summary,
                ActiveStudents = activeCards
                    .OrderByDescending(c => c.NextSessionDate ?? c.LastSessionDate)
                    .ToList(),
                PreviousStudents = previousCards
                    .OrderByDescending(c => c.LastSessionDate)
                    .ToList()
            };
        }

        public async Task<StudentProgressDto?> GetStudentProgressAsync(
            string teacherId, string studentId)
        {
            var now = DateTime.UtcNow;

            var sessions = await _teacherRepository
                .GetStudentSessionsWithTeacherAsync(teacherId, studentId);

            if (!sessions.Any()) return null;

            var student = sessions.First().Student?.ApplicationUser;
            var subject = sessions.First().SessionRequest?.Subject?.name ?? "N/A";
            if (student == null) return null;

            var completedSessions = sessions
                .Where(s => s.FocusScore.HasValue)
                .ToList();

            var upcomingSessions = sessions
                .Where(s =>
                    s.SessionRequest.Status == Models.RequestStatus.Accepted &&
                    s.SessionRequest.FinalScheduledAt.HasValue &&
                    s.SessionRequest.FinalScheduledAt.Value > now &&
                    !s.FocusScore.HasValue)
                .ToList();

            var totalHours = Math.Round(
               sessions
               .Where(s => s.RecordingDuration > 0)
               .Sum(s => s.RecordingDuration) / 3600.0m,2);

            double? avgFocus = completedSessions.Any()
                ? Math.Round(completedSessions.Average(s => (double)s.FocusScore!.Value), 2)
                : null;

            // Focus trend — last 5 completed sessions
            var focusTrend = completedSessions
                .OrderByDescending(s => s.SessionRequest.FinalScheduledAt)
                .Take(5)
                .OrderBy(s => s.SessionRequest.FinalScheduledAt)
                .Select((s, index) => new SessionFocusTrendDto
                {
                    SessionNumber = index + 1,
                    Date = s.SessionRequest.FinalScheduledAt!.Value,
                    FocusScore = s.FocusScore!.Value
                })
                .ToList();

            // Session history
            var sessionHistory = sessions
                .OrderByDescending(s => s.SessionRequest.FinalScheduledAt)
                .Select(s =>
                {
                    var scheduledAt = s.SessionRequest.FinalScheduledAt!.Value;
                    var hours = s.RecordingDuration > 0
                                      ? s.RecordingDuration / 3600.0
                                      : 1.0;
                    var status = s.FocusScore.HasValue ? "Completed" :
                                      s.SessionRequest.Status == Models.RequestStatus.Accepted &&
                                      scheduledAt > now ? "Upcoming" : "Pending";

                    return new StudentSessionHistoryDto
                    {
                        SessionId = s.SessionId,
                        Date = scheduledAt,
                        Time = scheduledAt.ToString("h:mm tt"),
                        Duration = FormatDuration(hours),
                        FocusScore = s.FocusScore,
                        Status = status,
                        Summary = s.Summary
                    };
                })
                .ToList();

            // Upcoming sessions
            var upcomingDtos = upcomingSessions
                .Select(s =>
                {
                    var scheduledAt = s.SessionRequest.FinalScheduledAt!.Value;
                    var hours = s.RecordingDuration > 0
                                      ? s.RecordingDuration / 3600.0
                                      : 1.0;

                    return new StudentUpcomingSessionDto
                    {
                        SessionId = s.SessionId,
                        ScheduledAt = scheduledAt,
                        Time = scheduledAt.ToString("h:mm tt"),
                        Duration = FormatDuration(hours)
                    };
                })
                .ToList();

            return new StudentProgressDto
            {
                StudentId = studentId,
                StudentName = student.FirstName + " " + student.LastName,
                ProfilePhotoURL = student.ProfilePhotoURL,
                SubjectName = subject,
                TotalSessions = sessions.Count,
                TotalHours = totalHours,
                AverageFocusScore = avgFocus,
                FirstSessionDate = sessions.First().SessionRequest?.FinalScheduledAt,
                LastSessionDate = completedSessions.Any()
                                    ? completedSessions
                                      .OrderByDescending(s => s.SessionRequest.FinalScheduledAt)
                                      .First().SessionRequest?.FinalScheduledAt
                                    : null,
                FocusTrend = focusTrend,
                SessionHistory = sessionHistory,
                UpcomingSessions = upcomingDtos
            };
        }

        // ═══════════════════════════════════════════════════════
        // PRIVATE HELPERS
        // ═══════════════════════════════════════════════════════
        private static string GetTimeAgo(DateTime? date)
        {
            if (!date.HasValue) return "Never";

            var diff = DateTime.UtcNow - date.Value;

            if (diff.TotalDays < 1) return "Today";
            if (diff.TotalDays < 2) return "Yesterday";
            if (diff.TotalDays < 7) return $"{(int)diff.TotalDays} days ago";
            if (diff.TotalDays < 14) return "1 week ago";
            if (diff.TotalDays < 30) return $"{(int)(diff.TotalDays / 7)} weeks ago";
            if (diff.TotalDays < 60) return "1 month ago";
            return $"{(int)(diff.TotalDays / 30)} months ago";
        }

        private static string FormatDuration(double hours)
        {
            if (hours == 1.0) return "1 hour";
            if (hours < 1.0) return $"{hours * 60:0} minutes";
            return $"{hours:0.#} hours";
        }

    }
}
