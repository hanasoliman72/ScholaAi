using ScholaAi.DTOs.Calendar;
using ScholaAi.Repositories.Calendar;
using ScholaAi.Repositories.Base;
using ScholaAi.Services.Base;

namespace ScholaAi.Services.Calendar
{
    public class CalendarService : ICalendarService
    {
        private readonly ICalendarRepository _calendarRepo;

        public CalendarService(ICalendarRepository calendarRepo)
        {
            _calendarRepo = calendarRepo;
        }

        public async Task<StudentCalendarMonthDto> GetStudentMonthAsync(
            string studentId, int year, int month)
            => await _calendarRepo.GetStudentMonthAsync(studentId, year, month);

        public async Task<StudentCalendarDayDetailDto> GetStudentDayAsync(
            string studentId, DateTime date)
            => await _calendarRepo.GetStudentDayAsync(studentId, date);

        public async Task<StudentSessionNotesDto> GetSessionNotesAsync(
            string studentId, int sessionId)
            => await _calendarRepo.GetSessionNotesAsync(studentId, sessionId);

        public async Task<TeacherCalendarMonthDto> GetTeacherMonthAsync(
            string teacherId, int year, int month)
            => await _calendarRepo.GetTeacherMonthAsync(teacherId, year, month);

        public async Task<TeacherCalendarDayDetailDto> GetTeacherDayAsync(
            string teacherId, DateTime date)
            => await _calendarRepo.GetTeacherDayAsync(teacherId, date);

        public async Task<TeacherSessionAnalysisDto?> GetSessionAnalysisAsync(
            string teacherId, int sessionId)
            => await _calendarRepo.GetSessionAnalysisAsync(teacherId, sessionId);
    }
}