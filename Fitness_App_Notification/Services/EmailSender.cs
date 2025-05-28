using Fitness_App_Notification.Models;
using System.Net;
using System.Net.Mail;
namespace Fitness_App_Notification.Services;
public class EmailSender
{
 private readonly SmtpClient _smtpClient;
    private readonly string _fromAddress;

    public EmailSender(EmailSettings settings)
    {
        _fromAddress = settings.From;

        _smtpClient = new SmtpClient(settings.SmtpServer, settings.Port)
        {
            Credentials = new NetworkCredential(settings.Username, settings.Password),
            EnableSsl = true
        };
    }
    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var message = new MailMessage(_fromAddress, toEmail, subject, body);
        await _smtpClient.SendMailAsync(message);
        Console.WriteLine($"Email успешно отправлен на {toEmail}");
    }
}