namespace ScholaAi.Repositories.Base
{
    public interface IFileUploadService
    {
        Task<string> UploadFileAsync(IFormFile file, string folder);
    }
}
