using CommunicationService.Application.Interfaces.Services;
using CommunicationService.Infrastracture.Implementations.Services.Configurations;
using Grpc.Net.Client;
using Microsoft.Extensions.Options;
using UsersService;

namespace CommunicationService.Infrastracture.Implementations.Services
{
    public class GrpcFriendsService : IFriendsService
    {
        private readonly string _grpcServerUrl;

        public GrpcFriendsService(IOptions<GrpcSettings> options)
        {
            _grpcServerUrl = options.Value.GrpcServerUrl;
        }

        public async Task<bool> AreFriendsAsync(
            string firstLogin,
            string secondLogin,
            CancellationToken cancellationToken = default
        )
        {
            var channel = GrpcChannel.ForAddress(_grpcServerUrl);
            var client = new Auth.AuthClient(channel);

            var request = new FriendsRequest { FirstLogin = firstLogin, SecondLogin = secondLogin };

            try
            {
                var response = await client.AreUsersFriendAsync(request, cancellationToken: cancellationToken);

                return response.AreFriend;
            }
            catch
            {
                return true;
            }
        }
    }
}
