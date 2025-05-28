namespace Fitness_App_Notification.Models;
public enum FriendshipStatus
{
    Pending,
    Accepted,
    Rejected
}

public enum ChallengeStatus
{
    Pending,
    Accepted,
    Completed,
    Failed,
    Rejected
}
public class NotificationMessage
{
    public string Type { get; set; } = "";
    public string SenderName { get; set; } = "";
    public string RecipientEmail { get; set; } = "";
    public string? Action { get; set; }
}
