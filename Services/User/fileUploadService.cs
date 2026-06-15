using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using ScholaAi.Services.Base;

namespace ScholaAi.Services.User
{
    public class fileUploadService : IFileUploadService
    {
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;

        public fileUploadService(
            IWebHostEnvironment env,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration config,
            HttpClient httpClient)
        {
            _env = env ?? throw new ArgumentNullException(nameof(env));
            _httpContextAccessor = httpContextAccessor;
            _config = config;
            _httpClient = httpClient;
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

        // for session recordings → Supabase
        public async Task<string?> UploadToSupabaseAsync(IFormFile file, string folder)
        {
            if (file == null || file.Length == 0)
                return null;

            var supabaseUrl = _config["Supabase:Url"];
            var anonKey = _config["Supabase:ServiceKey"];

            string fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            string storagePath = $"{folder}/{fileName}";
            string uploadUrl = $"{supabaseUrl}/storage/v1/object/{storagePath}";

            using var stream = file.OpenReadStream();
            using var content = new StreamContent(stream);
            var rawContentType = file.ContentType ?? "video/webm";
            var baseContentType = rawContentType.Split(';')[0].Trim();
            content.Headers.ContentType =
                new System.Net.Http.Headers.MediaTypeHeaderValue(baseContentType);

            var requestMsg = new HttpRequestMessage(HttpMethod.Post, uploadUrl)
            {
                Content = content
            };
            requestMsg.Headers.Add("Authorization", $"Bearer {anonKey}");
            requestMsg.Headers.Add("x-upsert", "true");

            var response = await _httpClient.SendAsync(requestMsg);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Supabase upload failed: {error}");
            }

            // return the public URL
            return $"{supabaseUrl}/storage/v1/object/public/{storagePath}";
        }
    }
}