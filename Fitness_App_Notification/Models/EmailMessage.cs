namespace Fitness_App_Notification.Models
{
    public class EmailMessage
    {
        public string Type { get; set; } = string.Empty; // "UserRegistered", "WorkoutCompleted" и т.д.
        public string Email { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
    }
}
