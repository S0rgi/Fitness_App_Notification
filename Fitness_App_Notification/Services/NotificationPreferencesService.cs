using Fitness_App_Notification.Data;
using Fitness_App_Notification.Grpc;
using Fitness_App_Notification.Models;
using Fitness_App_Notification.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fitness_App_Notification.Services
{
    public class NotificationPreferencesService : INotificationPreferencesService
    {
        private readonly NotificationDbContext _dbContext;

        public NotificationPreferencesService(NotificationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<NotificationPreferences> GetOrCreatePreferencesAsync(string email)
        {
            var pref = await _dbContext.Preferences
                .FirstOrDefaultAsync(p => p.Email == email);

            if (pref == null)
            {
                pref = new NotificationPreferences { Email = email };
                _dbContext.Preferences.Add(pref);
                await _dbContext.SaveChangesAsync();
            }
            return pref;
        }

        public async Task<NotificationPreferences> UpdatePreferencesAsync(string email, UpdateNotificationPreferencesRequest request)
        {
            var pref = await _dbContext.Preferences
                .FirstOrDefaultAsync(p => p.Email == email);

            if (pref == null)
            {
                pref = new NotificationPreferences { Email = email };
                _dbContext.Preferences.Add(pref);
            }

            if (request.FriendInvite.HasValue) pref.FriendInvite = request.FriendInvite.Value;
            if (request.FriendResponse.HasValue) pref.FriendResponse = request.FriendResponse.Value;
            if (request.ChallengeInvite.HasValue) pref.ChallengeInvite = request.ChallengeInvite.Value;
            if (request.ChallengeResponse.HasValue) pref.ChallengeResponse = request.ChallengeResponse.Value;

            await _dbContext.SaveChangesAsync();
            return pref;
        }
    }
}

