using GameService.Application.Features.Lobby.Commands.CreateLobby;
using GameService.Application.Interfaces.Repositories;
using GameService.Application.Interfaces.Services;
using GameService.Infrastructure.Implementations.Grpc;
using GameService.Infrastructure.Implementations.LobbyService;
using GameService.Infrastructure.Implementations.Notifications;
using GameService.Infrastructure.Persistense.Mongo;
using GameService.Infrastructure.Persistense.Mongo.Repositories;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace GameService.Presentation
{
    public static class DependencyInjectionExtensions
    {
        public static void AddMappers(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(CreateLobbyCommand).Assembly);
        }

        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<ILobbyRepository, LobbyRepository>();
        }

        public static void AddMongoDb(this IServiceCollection services, IConfiguration configuration) 
        {
            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

            services.AddSingleton<IMongoClient>(_ =>
            {
                return new MongoClient(
                    configuration.GetConnectionString("Mongo")
                        ?? throw new Exception("Missing Mongo connection string")
                );
            });

            services.Configure<MongoSettings>(configuration.GetSection("Mongo"));
        }

        public static void AddMediatR(this IServiceCollection services)
        {
            services.AddMediatR(configuration => configuration.RegisterServicesFromAssemblyContaining<CreateLobbyCommand>());
        }
        public static void ConfigureGrpc(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<GrpcSettings>(configuration.GetSection("Grpc"));
        }

        public static void ConfigureSingalR(this IServiceCollection services)
        {
            services.AddSignalR();

            services.AddScoped<ILobbyService, LobbyService>();

            services.AddScoped<IConfirmationService, ConfirmationService>();
        }

        public static void ConfigureRabbitMQ(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<NotificationSettings>(configuration.GetSection("RabbitMQ"));

            services.AddScoped<INotificationSender, RabbitNotificationSender>();
        }

        public static void MapSignalR(this WebApplication app)
        {
            app.MapHub<LobbyHub>("/lobbyHub");
        }
    }
}
