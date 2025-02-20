using System.Net.Mail;
using System.Net;

namespace Utility
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("omartalaat627@gmail.com", "exkr jbhy tdbj fffr")
            };

            return client.SendMailAsync(
                new MailMessage(from: "omartalaat627@gmail.com",
                                to: email,
                                subject,
                                message
                                )
                {
                    IsBodyHtml = true
                });
        }
    }
}
