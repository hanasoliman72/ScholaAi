using ScholaAi.DTOs.Admin;
using ScholaAi.Models;

namespace ScholaAi.Repositories.Base
{
    public interface IAdminRepository
    {
        // ─── Dashboard ─────────────────────────────────────────
        Task<AdminDashboardDto> GetDashboardStatsAsync();

        // ─── Users ─────────────────────────────────────────────
        Task<List<ApplicationUser>> GetAllUsersAsync(string? search, string? role);
        Task<ApplicationUser?> GetUserDetailAsync(string userId);
        Task<bool> DeleteUserAsync(string userId);
        Task<bool> ChangeUserRoleAsync(string userId, string newRole);
        Task SuspendUserAsync(string userId, int days);
        Task UnsuspendUserAsync(string userId);

        // ─── Sessions ──────────────────────────────────────────
        Task<List<AdminSessionListDto>> GetAllSessionsAsync(string? search);
        Task<AdminSessionDetailDto?> GetSessionDetailAsync(int sessionId);
        Task<List<AdminSessionListDto>> GetLiveSessionsAsync();

        // ─── Payments ──────────────────────────────────────────
        Task<List<AdminPaymentListDto>> GetAllPaymentsAsync(string? search);
        Task<AdminPaymentListDto?> GetPaymentDetailAsync(int transactionId);

        // ─── Subjects ──────────────────────────────────────────
        Task<List<AdminSubjectDto>> GetAllSubjectsAsync();
        Task<Subject> CreateSubjectAsync(CreateSubjectDto dto);
        Task<bool> UpdateSubjectAsync(int subjectId, UpdateSubjectDto dto);
        Task<bool> DeleteSubjectAsync(int subjectId);
    }
}