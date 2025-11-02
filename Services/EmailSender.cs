using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using AlmacenWeb.Models;


namespace AlmacenWeb.Services
{
    // Esta es la implementación REAL
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;

        public EmailSender(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            // Configurar el cliente SMTP para Gmail
            var client = new SmtpClient(_emailSettings.Host, _emailSettings.Port)
            {
                Credentials = new NetworkCredential(_emailSettings.Email, _emailSettings.Password),
                EnableSsl = _emailSettings.EnableSsl
            };

            // Crear el mensaje
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_emailSettings.Email, "AlmacenWeb Sistema"),
                Subject = subject,
                Body = message,
                IsBodyHtml = true
            };
            mailMessage.To.Add(email);

            // Enviar el correo
            await client.SendMailAsync(mailMessage);
        }
    }
}
