using System.ComponentModel.DataAnnotations; 
namespace Fitness_App_Notification.Models;

public class NotificationPreferences
{
    [Key]
    public string Email { get; set; } = null!;

    public bool FriendInvite { get; set; } = true;
    public bool FriendResponse { get; set; } = true;
    public bool ChallengeInvite { get; set; } = true;
    public bool ChallengeResponse { get; set; } = true;
}
