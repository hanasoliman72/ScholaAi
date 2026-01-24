using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using ScholaAi.Services.Base;

namespace ScholaAi.Services.User
{
    public class fileUploadService : IFileUploadService
    {
        private readonly IWebHostEnvironment _env;

        public fileUploadService(IWebHostEnvironment env)
        {
            _env = env ?? throw new ArgumentNullException(nameof(env));
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

            return $"/{folder}/{fileName}";
        }
    }

}
