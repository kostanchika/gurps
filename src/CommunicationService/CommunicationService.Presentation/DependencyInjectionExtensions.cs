using CommunicationService.Infrastracture.Implementations.Services.Configurations;

namespace CommunicationService.Presentation
{
    public static class DependencyInjectionExtensions
    {
        public static void ConfigureGrpc(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<GrpcSettings>(configuration.GetSection("Users"));
        }
    }
}
