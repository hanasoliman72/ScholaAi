namespace ScholaAi.Services.Base
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
    }
}
