using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Fitness_App_Notification.Models;
using Fitness_App_Notification.Data;
using Fitness_App_Notification.Grpc;
namespace Fitness_App_Notification.Controllers;

[ApiController]
[Route("api/notifications/preferences")]
public class NotificationPreferencesController : ControllerBase
{
    private readonly NotificationDbContext _dbContext;

    public NotificationPreferencesController(NotificationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // GET /api/notifications/preferences?email=user@example.com
    [HttpGet]
    [GrpcAuthorize]
    public async Task<IActionResult> GetPreferences()
    {
        var user = HttpContext.Items["User"] as UserResponse;
        var email = user.Email;
        if (string.IsNullOrWhiteSpace(email))
            return BadRequest("Email обязателен");

        var pref = await _dbContext.Preferences
            .FirstOrDefaultAsync(p => p.Email == email);

        if (pref == null)
        {
            pref = new NotificationPreferences { Email = email };
            _dbContext.Preferences.Add(pref);
            await _dbContext.SaveChangesAsync();
        }

        return Ok(pref);
    }

    // PATCH /api/notifications/preferences?email=user@example.com
    [HttpPatch]
    [GrpcAuthorize]
    public async Task<IActionResult> UpdatePreferences( [FromBody] UpdateNotificationPreferencesRequest request)
    {
        var user = HttpContext.Items["User"] as UserResponse;
        var email = user.Email;
        if (string.IsNullOrWhiteSpace(email))
            return BadRequest("Email обязателен");

        var pref = await _dbContext.Preferences
            .FirstOrDefaultAsync(p => p.Email == email);

        if (pref == null)
        {
            pref = new NotificationPreferences { Email = email };
            _dbContext.Preferences.Add(pref);
        }

        // Обновляем только те поля, которые указаны
        if (request.FriendInvite.HasValue) pref.FriendInvite = request.FriendInvite.Value;
        if (request.FriendResponse.HasValue) pref.FriendResponse = request.FriendResponse.Value;
        if (request.ChallengeInvite.HasValue) pref.ChallengeInvite = request.ChallengeInvite.Value;
        if (request.ChallengeResponse.HasValue) pref.ChallengeResponse = request.ChallengeResponse.Value;

        await _dbContext.SaveChangesAsync();

        return Ok(pref);
    }
}
