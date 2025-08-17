using Fitness_App_Notification.Models;
using Fitness_App_Notification.Services;
using Microsoft.Extensions.Options;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Fitness_App_Notification.Data;
using Microsoft.OpenApi.Models;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Fitness_App_Notification.Grpc;
using Fitness_App_Notification.Services.Interfaces;

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
builder.Services.AddHostedService<RabbitMqListener>(); 
builder.Services.AddScoped<INotificationPreferencesService, NotificationPreferencesService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Fitness App Notification", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Введите JWT токен",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme { Reference = new OpenApiReference {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer" }
            },
            new string[] {}
        }
    });
});
// Настройка GrpcChannel
var grpcHandler = new GrpcWebHandler(GrpcWebMode.GrpcWebText, new HttpClientHandler());
var grpcChannel = GrpcChannel.ForAddress(builder.Configuration.GetConnectionString("Grpc_Server"), new GrpcChannelOptions
{
    HttpHandler = grpcHandler
});

builder.Services.AddSingleton(new UserService.UserServiceClient(grpcChannel));
builder.WebHost.UseUrls("http://0.0.0.0:8080");
var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthorization();
app.MapControllers();

app.Run();
