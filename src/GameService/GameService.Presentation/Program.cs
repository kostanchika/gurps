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

            builder.Services.AddMediatR();
            builder.Services.AddMappers();
            builder.Services.AddMongoDb(builder.Configuration);
            builder.Services.AddRepositories();
            builder.Services.ConfigureRabbitMQ(builder.Configuration);
            builder.Services.ConfigureSingalR();
            builder.Services.ConfigureGrpc(builder.Configuration);

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
