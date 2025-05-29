using CommunicationService.Infrastracture.Implementations.ChatService;
using CommunicationService.Presentation.Middlewares;
using UsersService.Presentation.Middlewares;

namespace CommunicationService.Presentation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddPersistense(builder.Configuration);
            builder.Services.AddMediatR();
            builder.Services.AddMapping();
            builder.Services.AddValidation();
            builder.Services.ConfigureGrpc(builder.Configuration);
            builder.Services.ConfigureSignalR();

            builder.Services.AddControllers();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = null;
                options.DefaultChallengeScheme = null;
            });

            builder.Services.AddScoped<AuthMiddleware>();
            builder.Services.AddScoped<ExceptionHandlingMiddleware>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseMiddleware<ExceptionHandlingMiddleware>();

            app.UseMiddleware<AuthMiddleware>();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseWebSockets();


            app.MapHub<ChatHub>("/chatHub");

            app.MapControllers();

            app.Run();
        }
    }
}
