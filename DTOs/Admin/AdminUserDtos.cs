
using System.ComponentModel.DataAnnotations;

namespace ScholaAi.DTOs.Admin
{
    // Used in the list (GET all users)
    public class AdminUserListDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
        public bool IsSuspended { get; set; }
        public DateTime? SuspendedUntil { get; set; }
    }

    // Used in GET single user (extends the list dto)
    public class AdminUserDetailDto : AdminUserListDto
    {
        public string? PhoneNumber { get; set; }
        public string? Description { get; set; }
        public string? ProfilePhotoURL { get; set; }
        public string Gender { get; set; }

        // Student-specific
        public int? Grade { get; set; }

        // Teacher-specific
        public string? College { get; set; }
        public string? Certificate { get; set; }
        public string? TeachingExperience { get; set; }
        public string? Subject { get; set; }
        public decimal? TotalHoursTaught { get; set; }
        public decimal? AverageRating { get; set; }
    }

    // Used in POST create user
    public class AdminCreateUserDto
    {
        [Required] public string UserName { get; set; }
        [Required, EmailAddress] public string Email { get; set; }
        [Required, MinLength(6)] public string Password { get; set; }
        [Required] public string FirstName { get; set; }
        [Required] public string LastName { get; set; }
        [Required] public string Role { get; set; }
        public string? PhoneNumber { get; set; }
        public string Gender { get; set; } = "Male";

        // Student-specific
        public int? Grade { get; set; }

        // Teacher-specific
        public string? College { get; set; }
        public string? Certificate { get; set; }
        public string? TeachingExperience { get; set; }
        public int? SubjectId { get; set; }
    }

    // Used in PUT edit user
    public class AdminEditUserDto
    {
        public string? UserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Description { get; set; }

        // Student-specific
        public int? Grade { get; set; }

        // Teacher-specific
        public string? College { get; set; }
        public string? Certificate { get; set; }
        public string? TeachingExperience { get; set; }
    }

    // Used in PUT change role
    public class ChangeUserRoleDto
    {
        [Required] public string NewRole { get; set; }
    }

    // Used in POST suspend
    public class SuspendUserDto
    {
        [Required] public int DurationInDays { get; set; }
        public string? Reason { get; set; }
    }
}