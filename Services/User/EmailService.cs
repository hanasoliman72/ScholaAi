using ScholaAi.Services.Base;

namespace ScholaAi.Services.User
{
    public class EmailService : IEmailService
    {
        public async Task SendEmailAsync(string to, string subject, string body)
        {
           
            Console.WriteLine($"Sending email to {to}");
            Console.WriteLine($"Subject: {subject}");
            Console.WriteLine($"Body: {body}");

            await Task.CompletedTask;
        }
    }

}
