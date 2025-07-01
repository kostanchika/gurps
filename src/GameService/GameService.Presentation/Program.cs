using Serilog;
using Serilog.Sinks.Network;
using GameService.Application.Interfaces.Services;
using GameService.Presentation.Middlewares;
using GURPS.Character.Providers.Configuration;
using GURPS.Character.Providers.Implementations.Providers.JSON;
using GURPS.Character.Providers.Implementations;
using GURPS.Character.Providers.Interfaces;
using GameService.Infrastructure.Implementations;

namespace GameService.Presentation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var elasticUri = builder.Configuration["Elastic:Uri"]
                ?? throw new Exception("Elastic section is missing or bad configured");

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithEnvironmentName()
                .Enrich.WithThreadId()
                .WriteTo.Console()
                .WriteTo.TCPSink(elasticUri)
                .ReadFrom.Configuration(builder.Configuration)
                .CreateLogger();

            builder.Host.UseSerilog();
            
            builder.Services.AddMediatR();
            builder.Services.AddMappers();
            builder.Services.AddMongoDb(builder.Configuration);
            builder.Services.AddRepositories();
            builder.Services.ConfigureRabbitMQ(builder.Configuration);
            builder.Services.ConfigureSingalR();
            builder.Services.ConfigureGrpc(builder.Configuration);

            builder.Services.AddScoped<IImageService, PNGImageService>();
            builder.Services.AddScoped<ICharacterManager, CharacterManager>();
            builder.Services.AddScoped<ICharacterCalculator, CharacterCalculator>();
            builder.Services.AddSingleton<ICharacterConfigurationProvider, JSONCharacterConfigurationProvider>(
                (serviceProvider) =>
                {
                    var characterSettings = builder.Configuration.GetSection("Character").Get<CharacterSettings>()
                        ?? throw new Exception("Character section is missing or bad configured");

                    return new JSONCharacterConfigurationProvider(characterSettings.SettingsPath);
                }
            );

            builder.Services.AddControllers();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = null;
                options.DefaultChallengeScheme = null;
            });

            builder.Services.AddScoped<ExceptionHandlingMiddleware>();
            builder.Services.AddScoped<AuthMiddleware>();

            var app = builder.Build();

            app.UseCors(options =>
            {
                options.WithOrigins(builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? []);
                options.AllowAnyHeader();
                options.AllowAnyMethod();
                options.AllowCredentials();
                options.WithExposedHeaders("X-Total-Count");
            });

            app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.UseMiddleware<AuthMiddleware>();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseWebSockets();


            app.MapSignalR();
            app.MapControllers();

            app.Run();
        }
    }
}
