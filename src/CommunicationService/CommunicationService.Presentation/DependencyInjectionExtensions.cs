using CommunicationService.Infrastracture.Implementations.Services.Configurations;
using FluentValidation.AspNetCore;
using FluentValidation;

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
        public static void ConfigureGrpc(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<GrpcSettings>(configuration.GetSection("Users"));
        }
    }
}
