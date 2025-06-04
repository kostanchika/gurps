using CommunicationService.Application.Features.Chat.Commands.SendMessage;
using CommunicationService.Application.Interfaces.Repositories;
using CommunicationService.Application.Interfaces.Services;
using CommunicationService.Infrastracture.Implementations.ChatService;
using CommunicationService.Infrastracture.Implementations.Services;
using CommunicationService.Infrastracture.Implementations.Services.Configurations;
using CommunicationService.Infrastracture.Persistense.Mongo;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver;
using CommunicationService.Application.Features.Chat.Commands.CreateChat;
using FluentValidation.AspNetCore;
using FluentValidation;
using CommunicationService.Infrastracture.Persistense.Mongo.Configurations;
using CommunicationService.Infrastracture.Implementations.NotificationService;

namespace CommunicationService.Presentation
{
    public static class DependencyInjectionExtensions
    {
        public static void AddMediatR(this IServiceCollection services)
        {
            services.AddMediatR(configuration => configuration.RegisterServicesFromAssemblyContaining<SendMessageCommand>());
        }

        public static void AddMapping(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(SendMessageCommand).Assembly);
        }

        public static void AddValidation(this IServiceCollection services)
        {
            services.AddFluentValidationAutoValidation();

            services.AddValidatorsFromAssemblyContaining(typeof(CreateChatValidator));
        }

        public static void AddPersistense(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IMongoClient>(_ =>
            {
                return new MongoClient(
                    configuration.GetConnectionString("MongoConnection")
                        ?? throw new Exception("Missing Mongo connection string")
                );
            });

            services.Configure<MongoSettings>(configuration.GetSection("Mongo"));

            services.AddScoped<IChatRepository, ChatRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
        }

        public static void AddLocalAttachments(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AttachmentSettings>(configuration.GetSection("Attachment"));

            services.AddScoped<IAttachmentService, LocalAttachmentService>();
        }

        public static void ConfigureGrpc(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<GrpcSettings>(configuration.GetSection("Users"));

            services.AddScoped<IFriendsService, GrpcFriendsService>();
        }

        public static void ConfigureSignalR(this IServiceCollection services)
        {
            services.AddSignalR();

            services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();
            services.AddSingleton<IConnectionMapper, ConnectionMapper>();

            services.AddScoped<IChatService, ChatService>();
            services.AddScoped<INotificationService, NotificationService>();
        }

        public static void ConfigureRabbitMQ(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<NotificationSettings>(configuration.GetSection("RabbitMQ"));

            services.AddHostedService<NotificationBackgroundConsumer>();
        }
    }
}
