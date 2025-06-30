using Serilog;
using Serilog.Sinks.Network;

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

            // Add services to the container.

            builder.Services.AddControllers();

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
