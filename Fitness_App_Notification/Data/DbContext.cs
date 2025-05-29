using Microsoft.EntityFrameworkCore;
using Fitness_App_Notification.Models;
namespace Fitness_App_Notification.Data;

public class NotificationDbContext : DbContext
{
    public NotificationDbContext(DbContextOptions<NotificationDbContext> options)
        : base(options) { }

    public DbSet<NotificationPreferences> Preferences { get; set; } = null!;
}
