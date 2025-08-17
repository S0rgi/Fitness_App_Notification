using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Fitness_App_Notification.Models;
using Fitness_App_Notification.Data;
using Fitness_App_Notification.Grpc;
using Fitness_App_Notification.Services.Interfaces;

namespace Fitness_App_Notification.Controllers;

[ApiController]
[Route("api/notifications/preferences")]
public class NotificationPreferencesController : ControllerBase
{
    private readonly INotificationPreferencesService _notificationPreferencesService;

    public NotificationPreferencesController(INotificationPreferencesService notificationPreferencesService)
    {
        _notificationPreferencesService = notificationPreferencesService;
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

        var pref = await _notificationPreferencesService.GetOrCreatePreferencesAsync(email);
        return Ok(pref);
    }

    // PATCH /api/notifications/preferences?email=user@example.com
    [HttpPatch]
    [GrpcAuthorize]
    public async Task<IActionResult> UpdatePreferences([FromBody] UpdateNotificationPreferencesRequest request)
    {
        var user = HttpContext.Items["User"] as UserResponse;
        var email = user.Email;
        if (string.IsNullOrWhiteSpace(email))
            return BadRequest("Email обязателен");

        var pref = await _notificationPreferencesService.UpdatePreferencesAsync(email, request);

        return Ok(pref);
    }
}
