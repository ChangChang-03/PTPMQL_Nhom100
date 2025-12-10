using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

namespace MVC.Services
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Không gửi thật, chỉ log ra console
            Console.WriteLine("Email To: " + email);
            Console.WriteLine("Subject: " + subject);
            Console.WriteLine("Message: " + htmlMessage);

            return Task.CompletedTask;
        }
    }
}
