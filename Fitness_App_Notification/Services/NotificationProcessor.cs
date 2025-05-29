using Newtonsoft.Json;
using Fitness_App_Notification.Models;
using Microsoft.EntityFrameworkCore;
namespace Fitness_App_Notification.Services;

using Fitness_App_Notification.Data;

public class NotificationProcessor
{
    private readonly EmailSender _emailSender;
    private readonly NotificationDbContext _dbContext;

    public NotificationProcessor(EmailSender emailSender, NotificationDbContext dbContext)
    {
        _emailSender = emailSender;
        _dbContext = dbContext;
    }

public async Task HandleMessageAsync(string json)
{
    try
    {
        var notification = JsonConvert.DeserializeObject<NotificationMessage>(json);
        if (notification == null)
        {
            Console.WriteLine(" [!] Пустое или некорректное сообщение");
            return;
        }

        var preferences = await GetOrCreatePreferences(notification.RecipientEmail);

        var formatted = FormatEmail(notification, preferences);
        if (formatted == null)
        {
            Console.WriteLine($" [x] Уведомление '{notification.Type}' не отправлено — отключено у {notification.RecipientEmail}");
            return;
        }

        var (subject, body) = formatted.Value;
        await _emailSender.SendEmailAsync(notification.RecipientEmail, subject, body);
        Console.WriteLine($" [x] Email отправлен на {notification.RecipientEmail}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($" [!] Ошибка обработки сообщения: {ex.Message}");
    }
}


    private async Task<NotificationPreferences> GetOrCreatePreferences(string email)
    {
        var pref = await _dbContext.Preferences
            .FirstOrDefaultAsync(p => p.Email == email);

        if (pref == null)
        {
            pref = new NotificationPreferences { Email = email };
            _dbContext.Preferences.Add(pref);
            await _dbContext.SaveChangesAsync();
            Console.WriteLine($" [+] Настройки по умолчанию созданы для {email}");
        }

        return pref;
    }


private (string subject, string body)? FormatEmail(NotificationMessage msg, NotificationPreferences pref)
{
    string subject = "";
    string body = $"Привет!\n\n";

    switch (msg.Type)
    {
        case "friend_invite":
            if (!pref.FriendInvite) return null;
            subject = "Уведомление от Fitness App";
            body += $"{msg.SenderName} отправил вам приглашение в друзья.";
            break;

        case "friend_response":
            if (!pref.FriendResponse) return null;
            subject = "Уведомление от Fitness App";

            if (TryParseFriendshipStatus(msg.Action, out var friendStatus))
            {
                body += friendStatus switch
                {
                    FriendshipStatus.Accepted => $"{msg.SenderName} ответил на ваше приглашение.",
                    FriendshipStatus.Rejected => $"{msg.SenderName} отклонил ваше приглашение.",
                    _ => $"{msg.SenderName} обновил статус вашего приглашения."
                };
            }
            else
            {
                body += $"{msg.SenderName} ответил на ваше приглашение.";
            }
            break;

        case "challenge_invite":
            if (!pref.ChallengeInvite) return null;
            subject = "Новый вызов в Fitness App";
            body += $"{msg.SenderName} отправил вам челлендж.";
            break;

        case "challenge_response":
            if (!pref.ChallengeResponse) return null;
            subject = "Новый вызов в Fitness App";

            if (TryParseChallengeStatus(msg.Action, out var challengeStatus))
            {
                body += challengeStatus switch
                {
                    ChallengeStatus.Accepted => $"{msg.SenderName} ответил на ваш челлендж.",
                    ChallengeStatus.Rejected => $"{msg.SenderName} отклонил ваш челлендж.",
                    ChallengeStatus.Completed => $"{msg.SenderName} завершил челлендж.",
                    ChallengeStatus.Failed => $"{msg.SenderName} не выполнил челлендж.",
                    _ => $"{msg.SenderName} обновил статус вашего челленджа."
                };
            }
            else
            {
                body += $"{msg.SenderName} ответил на ваш челлендж.";
            }
            break;

        default:
            subject = "Уведомление от Fitness App";
            body += "У вас новое уведомление.";
            break;
    }

    body += "\n\nFitness App";

    return (subject, body);
}

    private bool TryParseFriendshipStatus(string? action, out FriendshipStatus status)
    {
        if (string.IsNullOrEmpty(action))
        {
            status = FriendshipStatus.Pending;
            return false;
        }

        return Enum.TryParse<FriendshipStatus>(ToPascalCase(action), out status);
    }

    private bool TryParseChallengeStatus(string? action, out ChallengeStatus status)
    {
        if (string.IsNullOrEmpty(action))
        {
            status = ChallengeStatus.Pending;
            return false;
        }

        return Enum.TryParse<ChallengeStatus>(ToPascalCase(action), out status);
    }

    private string ToPascalCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input ?? "";

        return char.ToUpperInvariant(input[0]) + input.Substring(1).ToLowerInvariant();
    }

}
