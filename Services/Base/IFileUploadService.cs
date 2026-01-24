using Microsoft.AspNetCore.Http;

namespace ScholaAi.Services.Base
{
    public interface IFileUploadService
    {
        Task<string?> UploadFileAsync(IFormFile file, string folder);
    }
}
