using GameService.Application.Interfaces.Services;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace GameService.Infrastructure.Implementations.Notifications
{
    public class RabbitNotificationSender : INotificationSender, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IChannel _channel;
        private readonly string _queue;

        public RabbitNotificationSender(
            IOptions<NotificationSettings> options
        )
        {
            var settings = options.Value;
            _queue = settings.Queue;

            var factory = new ConnectionFactory
            {
                HostName = settings.Host,
                UserName = settings.User,
                Password = settings.Password
            };

            _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
            _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();
            _channel.QueueDeclareAsync(settings.Queue, true, false, false, null).Wait();
        }

        public void Dispose()
        {
            _channel.CloseAsync().Wait();
            _connection.CloseAsync().Wait();
            GC.SuppressFinalize(this);
        }

        public async Task SendNotificationAsync(
            string recipient,
            string title,
            string content,
            Dictionary<string, string> additionalInfo,
            CancellationToken cancellationToken = default
        )
        {
            var payload = new
            {
                Login = recipient,
                Name = title,
                Content = content,
                Meta = additionalInfo.Select(c => new { Name = c.Key, Description = c.Value })
            };

            var json = JsonSerializer.Serialize(payload);
            var body = Encoding.UTF8.GetBytes(json);

            await _channel.BasicPublishAsync("", _queue, body: body, cancellationToken);
        }
    }
}
