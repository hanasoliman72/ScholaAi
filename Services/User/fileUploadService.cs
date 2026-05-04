using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using ScholaAi.Services.Base;

namespace ScholaAi.Services.User
{
    public class fileUploadService : IFileUploadService
    {
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public fileUploadService(IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            _env = env ?? throw new ArgumentNullException(nameof(env));
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string?> UploadFileAsync(IFormFile file, string folder)
        {
            if (file == null || file.Length == 0)
                return null;

            if (string.IsNullOrWhiteSpace(_env.WebRootPath))
                throw new InvalidOperationException("WebRootPath is not configured.");

            string uploadPath = Path.Combine(_env.WebRootPath, folder);

            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string filePath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var request = _httpContextAccessor.HttpContext!.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";
            return $"{baseUrl}/{folder}/{fileName}";
        }
    }

}
