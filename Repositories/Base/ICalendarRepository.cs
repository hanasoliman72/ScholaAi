using ScholaAi.DTOs.Calendar;

namespace ScholaAi.Repositories.Calendar
{
    public interface ICalendarRepository
    {
        // Student
        Task<StudentCalendarMonthDto> GetStudentMonthAsync(string studentId, int year, int month);
        Task<StudentCalendarDayDetailDto> GetStudentDayAsync(string studentId, DateTime date);
        Task<StudentSessionNotesDto?> GetSessionNotesAsync(string studentId, int sessionId);

        // Teacher
        Task<TeacherCalendarMonthDto> GetTeacherMonthAsync(string teacherId, int year, int month);
        Task<TeacherCalendarDayDetailDto> GetTeacherDayAsync(string teacherId, DateTime date);
        Task<TeacherSessionAnalysisDto?> GetSessionAnalysisAsync(string teacherId, int sessionId);
    }
}