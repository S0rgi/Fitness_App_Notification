using Fitness_App_Notification.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Microsoft.Extensions.Options;

namespace Fitness_App_Notification.Services;

public class RabbitMqListener : BackgroundService
{
    private readonly NotificationProcessor _processor;
    private readonly RabbitMqSettings _settings;
    private IConnection? _connection;
    private IChannel? _channel;

    public RabbitMqListener(NotificationProcessor processor, IOptions<RabbitMqSettings> options)
    {
        _processor = processor;
        _settings = options.Value;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _settings.HostName,
            UserName = _settings.UserName,
            Password = _settings.Password
        };

        _connection = await factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();

        await _channel.QueueDeclareAsync(queue: "notifications", durable: false, exclusive: false, autoDelete: false, arguments: null);

        Console.WriteLine(" [*] Подключение к очереди установлено. Ожидаю сообщения...");

        await base.StartAsync(cancellationToken);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($" [x] Получено сообщение: {message}");

            await _processor.HandleMessageAsync(message);
        };

        _channel.BasicConsumeAsync(queue: "notifications", autoAck: true, consumer: consumer);

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
