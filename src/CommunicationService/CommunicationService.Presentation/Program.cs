using CommunicationService.Presentation.Middlewares;
using UsersService.Presentation.Middlewares;

namespace CommunicationService.Presentation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddMediatR();
            builder.Services.AddMapping();
            builder.Services.AddValidation();
            builder.Services.ConfigureGrpc(builder.Configuration);

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

            app.UseMiddleware<AuthMiddleware>();
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
