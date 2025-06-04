using CommunicationService.Application.Features.Notification.Commands.SendNotification;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace CommunicationService.Infrastracture.Implementations.NotificationService
{
    public class NotificationBackgroundConsumer : BackgroundService
    {
        private readonly IConnection _connection;
        private readonly IChannel _channel;
        private readonly IMediator _mediator;
        private readonly string _queue;

        public NotificationBackgroundConsumer(
            IMediator mediator,
            IOptions<NotificationSettings> options
        )
        {
            _mediator = mediator;

            var settings = options.Value;
            _queue = settings.Queue;

            var factory = new ConnectionFactory
            {
                HostName = settings.Host,
                UserName = settings.User,
                Password = settings.Password
            };

            _connection = factory.CreateConnectionAsync().Result;
            _channel = _connection.CreateChannelAsync().Result;
            _channel.QueueDeclareAsync(settings.Queue, true, false, false, null).Wait();
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (model, eventArgs) =>
            {
                var body = eventArgs.Body.ToArray();
                var jsonString = Encoding.UTF8.GetString(body);
                var notification = JsonSerializer.Deserialize<NotificationMessage>(jsonString);

                if (notification != null)
                {
                    await _mediator.Send(
                        new SendNotificationCommand(notification.Login, notification.Name, notification.Content, notification.Meta),
                        cancellationToken
                    );
                }
            };

            await _channel.BasicConsumeAsync(_queue, true, consumer, cancellationToken);

            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(1000, cancellationToken);
            }
        }

        public override void Dispose()
        {
            _channel.CloseAsync().Wait();
            _connection.CloseAsync().Wait();
            base.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
