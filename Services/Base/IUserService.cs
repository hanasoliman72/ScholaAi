using ScholaAi.DTOs.Common;

namespace ScholaAi.Repositories.Base
{
    public interface IUserService
    {
        Task<bool> ChangePasswordAsync(int userId, changePasswordDto dto);
        Task<string?> UploadProfilePhotoAsync(int userId, IFormFile file);
    }
}
