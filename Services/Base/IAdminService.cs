//using ScholaAi.DTOs.Admin;

//namespace ScholaAi.Services.Base
//{

//    public interface IAdminService
//    {
//        // ─── Dashboard ─────────────────────────────────────────
//        Task<AdminDashboardDto> GetDashboardAsync();

//        // ─── Users ─────────────────────────────────────────────
//        Task<List<AdminUserListDto>> GetAllUsersAsync(string? search, string? role);
//        Task<AdminUserDetailDto?> GetUserDetailAsync(string userId);
//        Task<AdminUserDetailDto> CreateUserAsync(AdminCreateUserDto dto);
//        Task<bool> EditUserAsync(string userId, AdminEditUserDto dto);
//        Task<bool> DeleteUserAsync(string userId);
//        Task<bool> ChangeUserRoleAsync(string userId, ChangeUserRoleDto dto);
//        Task<bool> SuspendUserAsync(string userId, SuspendUserDto dto);
//        Task<bool> UnsuspendUserAsync(string userId);

//        // ─── Sessions ──────────────────────────────────────────
//        Task<List<AdminSessionListDto>> GetAllSessionsAsync(string? search);
//        Task<AdminSessionDetailDto?> GetSessionDetailAsync(int sessionId);
//        Task<List<AdminSessionListDto>> GetLiveSessionsAsync();

//        // ─── Payments ──────────────────────────────────────────
//        Task<List<AdminPaymentListDto>> GetAllPaymentsAsync(string? search);
//        Task<AdminPaymentListDto?> GetPaymentDetailAsync(int transactionId);
//        Task<byte[]> ExportPaymentsCsvAsync();

//        // ─── Subjects ──────────────────────────────────────────
//        Task<List<AdminSubjectDto>> GetAllSubjectsAsync();
//        Task<AdminSubjectDto> CreateSubjectAsync(CreateSubjectDto dto);
//        Task<bool> UpdateSubjectAsync(int subjectId, UpdateSubjectDto dto);
//        Task<bool> DeleteSubjectAsync(int subjectId);

//        // ─── Admin Profile ─────────────────────────────────────
//        Task<AdminProfileDto?> GetAdminProfileAsync(string adminId);
//        Task<bool> UpdateAdminProfileAsync(string adminId, AdminEditProfileDto dto);
//    }
//}

using ScholaAi.DTOs.Admin;

namespace ScholaAi.Services.Base
{
    public interface IAdminService
    {
        // ─── Dashboard ─────────────────────────────────────────
        Task<AdminDashboardDto> GetDashboardAsync();

        Task<List<AdminLogDto>> GetAdminLogsAsync();

        // ─── Users ─────────────────────────────────────────────
        Task<List<AdminUserListDto>> GetAllUsersAsync(string? search, string? role);
        Task<AdminUserDetailDto?> GetUserDetailAsync(string userId);
        Task<AdminUserDetailDto> CreateUserAsync(AdminCreateUserDto dto);
        Task<bool> EditUserAsync(string userId, AdminEditUserDto dto);
        Task<bool> DeleteUserAsync(string adminId, string userId);
        Task<bool> ChangeUserRoleAsync(string adminId, string userId, ChangeUserRoleDto dto);
        Task<bool> SuspendUserAsync(string adminId, string userId, SuspendUserDto dto);
        Task<bool> UnsuspendUserAsync(string adminId, string userId);


        // ─── Teacher Verification ──────────────────────────────
        Task<bool> VerifyTeacherAsync(string adminId, string teacherId, string? notes);
        Task<bool> UnverifyTeacherAsync(string adminId, string teacherId);
     

        // ─── Sessions ──────────────────────────────────────────
        Task<List<AdminSessionListDto>> GetAllSessionsAsync(string? search);
        Task<AdminSessionDetailDto?> GetSessionDetailAsync(int sessionId);
        Task<List<AdminSessionListDto>> GetLiveSessionsAsync();

        // ─── Payments ──────────────────────────────────────────
        Task<List<AdminPaymentListDto>> GetAllPaymentsAsync(string? search);
        Task<AdminPaymentListDto?> GetPaymentDetailAsync(int transactionId);
        Task<byte[]> ExportPaymentsCsvAsync();

        // ─── Ratings ───────────────────────────────────────────
        Task<List<AdminRatingDto>> GetAllRatingsAsync();

        // ─── Subjects ──────────────────────────────────────────
        Task<List<AdminSubjectDto>> GetAllSubjectsAsync();
        Task<AdminSubjectDto> CreateSubjectAsync(CreateSubjectDto dto);
        Task<bool> UpdateSubjectAsync(int subjectId, UpdateSubjectDto dto);
        Task<bool> DeleteSubjectAsync(int subjectId);

        // ─── Admin Profile ─────────────────────────────────────
        Task<AdminProfileDto?> GetAdminProfileAsync(string adminId);
        Task<bool> UpdateAdminProfileAsync(string adminId, AdminEditProfileDto dto);
        Task<bool> ChangeAdminPasswordAsync(string adminId, AdminChangePasswordDto dto);
    }
}