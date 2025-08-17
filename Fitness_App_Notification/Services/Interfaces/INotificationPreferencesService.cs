using Fitness_App_Notification.Models;
using Fitness_App_Notification.Grpc;

namespace Fitness_App_Notification.Services.Interfaces
{
    public interface INotificationPreferencesService
    {
        Task<NotificationPreferences> GetOrCreatePreferencesAsync(string email);
        Task<NotificationPreferences> UpdatePreferencesAsync(string email, UpdateNotificationPreferencesRequest request);
    }
}

