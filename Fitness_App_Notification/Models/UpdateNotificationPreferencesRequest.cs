namespace Fitness_App_Notification.Models;
public class UpdateNotificationPreferencesRequest
{
    public bool? FriendInvite { get; set; }
    public bool? FriendResponse { get; set; }
    public bool? ChallengeInvite { get; set; }
    public bool? ChallengeResponse { get; set; }
}
