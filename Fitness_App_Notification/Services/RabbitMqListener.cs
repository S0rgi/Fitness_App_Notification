using Fitness_App_Notification.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;

namespace Fitness_App_Notification.Services;

public class RabbitMqListener : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly string _connectionString;
    private IConnection? _connection;
    private IChannel? _channel;

    public RabbitMqListener(IServiceScopeFactory scopeFactory, IConfiguration configuration)
    {
        _scopeFactory = scopeFactory;
        _connectionString = configuration.GetConnectionString("RabbitMq");
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory
        {
            Uri = new Uri(_connectionString)
        };

        _connection = await factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();

        // Объявляем обе очереди
        await _channel.QueueDeclareAsync("notifications", durable: false, exclusive: false, autoDelete: false, arguments: null);
        await _channel.QueueDeclareAsync("code", durable: false, exclusive: false, autoDelete: false, arguments: null);

        Console.WriteLine(" [*] Подключение к очередям notifications и code установлено. Ожидаю сообщения...");

        await base.StartAsync(cancellationToken);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Консьюмер для notifications
        var notificationsConsumer = new AsyncEventingBasicConsumer(_channel);
        notificationsConsumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($" [x] Получено сообщение из 'notifications': {message}");

            using var scope = _scopeFactory.CreateScope();
            var processor = scope.ServiceProvider.GetRequiredService<NotificationProcessor>();
            await processor.HandleMessageAsync(message);
        };

        // Консьюмер для code
        var codeConsumer = new AsyncEventingBasicConsumer(_channel);
        codeConsumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($" [x] Получено сообщение из 'code': {message}");

            using var scope = _scopeFactory.CreateScope();
            var processor = scope.ServiceProvider.GetRequiredService<NotificationProcessor>();
            await processor.HandleMessageAsync(message); // Можно создать отдельный метод, если логика отличается
        };

        // Подписка на очереди
        _channel.BasicConsumeAsync(queue: "notifications", autoAck: true, consumer: notificationsConsumer);
        _channel.BasicConsumeAsync(queue: "code", autoAck: true, consumer: codeConsumer);

        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel != null)
            await _channel.CloseAsync();
        if (_connection != null)
            await _connection.CloseAsync();

        await base.StopAsync(cancellationToken);
    }

    public override void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        base.Dispose();
    }
}
