using CommunicationService.Infrastracture.Implementations.ChatService;
using CommunicationService.Infrastracture.Implementations.NotificationService;
using CommunicationService.Presentation.Middlewares;
using Serilog;
using Serilog.Sinks.Network;
using UsersService.Presentation.Middlewares;

namespace CommunicationService.Presentation
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

            builder.Services.AddPersistense(builder.Configuration);
            builder.Services.AddMediatR();
            builder.Services.AddMapping();
            builder.Services.AddValidation();
            builder.Services.AddLocalAttachments(builder.Configuration);
            builder.Services.ConfigureGrpc(builder.Configuration);
            builder.Services.ConfigureSignalR();
            builder.Services.ConfigureRabbitMQ(builder.Configuration);

            builder.Services.AddControllers();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = null;
                options.DefaultChallengeScheme = null;
            });

            builder.Services.AddScoped<AuthMiddleware>();
            builder.Services.AddScoped<ExceptionHandlingMiddleware>();

            var app = builder.Build();

            app.UseCors(options =>
            {
                options.WithOrigins(app.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? []);
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

            app.UseStaticFiles(new StaticFileOptions
            {
                ServeUnknownFileTypes = true
            });

            app.MapHub<ChatHub>("/chatHub");
            app.MapHub<NotificationHub>("/notificationHub");

            app.MapControllers();

            app.Run();
        }
    }
}
