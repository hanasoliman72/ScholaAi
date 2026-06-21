using System.Collections.Generic;
using ScholaAi.Models;

namespace ScholaAi.Repositories.Base
{
    public interface ISessionRepository
    {
        Task<Session?> GetByIdAsync(int sessionId);
        Task<List<Session>> GetByStudentIdAsync(string studentId);
        Task<Session?> GetByRequestIdAsync(int requestId);
        Task<bool> HasActiveSessionForTeacherAsync(string teacherId);
        Task<bool> HasActiveSessionForStudentAsync(string studentId);
        Task AddAsync(Session session);
        Task SaveAsync();
    }
}
