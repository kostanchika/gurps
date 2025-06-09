using CommunicationService.Application.Exceptions;
using CommunicationService.Infrastracture.Implementations.Services.Configurations;
using Grpc.Net.Client;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using UsersService;

namespace CommunicationService.Presentation.Middlewares
{
    public class AuthMiddleware : IMiddleware
    {
        private readonly string _grpcServerUrl;

        public AuthMiddleware(IOptions<GrpcSettings> options)
        {
            _grpcServerUrl = options.Value.GrpcServerUrl;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var token = context.Request.Headers.Authorization.FirstOrDefault()?.Replace("Bearer ", "")
                ?? context.Request.Query["access_token"];

            if (!string.IsNullOrEmpty(token))
            {
                var channel = GrpcChannel.ForAddress(_grpcServerUrl);
                var client = new Auth.AuthClient(channel);

                var request = new TokenRequest { Token = token };

                try
                {
                    var response = await client.ValidateTokenAsync(request);

                    if (string.IsNullOrEmpty(response.Login))
                    {
                        throw new UnauthorizedException("");
                    }

                    var claims = new List<Claim>
                    {
                        new (ClaimTypes.NameIdentifier, response.Login)
                    };

                    var identity = new ClaimsIdentity(claims, "grpc");
                    context.User = new ClaimsPrincipal(identity);
                }
                catch
                {
                    throw new UnauthorizedException("Unauthorized access");
                }
            }

            await next(context);
        }
    }
}
