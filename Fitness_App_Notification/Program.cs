using Fitness_App_Notification.Models;
using Fitness_App_Notification.Services;
using Microsoft.Extensions.Options;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Fitness_App_Notification.Data;
DotNetEnv.Env.Load("../.env");

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddSingleton<EmailSender>(sp =>
{
    var emailSettings = sp.GetRequiredService<IOptions<EmailSettings>>().Value;
    return new EmailSender(emailSettings);
});
builder.Services.AddDbContext<NotificationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<NotificationProcessor>();
builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMq"));
builder.Services.AddHostedService<RabbitMqListener>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.Run();
