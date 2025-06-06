using Grpc.Core;
using UsersService.Application.DTOs.Friend;
using UsersService.Application.Interfaces.Services;
using UsersService.Application.Interfaces.UseCases.Friend;

namespace UsersService.Infrastructure.Services
{
    public class AuthService : Auth.AuthBase
    {
        private readonly ITokenService _tokenService;
        private readonly IGetActiveFriendsUseCase _getActiveFriendsUseCase;

        public AuthService(
            ITokenService tokenService,
            IGetActiveFriendsUseCase getActiveFriendsUseCase
        )
        {
            _tokenService = tokenService;
            _getActiveFriendsUseCase = getActiveFriendsUseCase;
        }

        public override Task<UserResponse> ValidateToken(TokenRequest request, ServerCallContext context)
        {
            return Task.FromResult(
                new UserResponse
                {
                    Login = _tokenService.GetLoginFromToken(request.Token) ?? ""
                });
        }

        public override async Task<FriendsResponse> AreUsersFriend(FriendsRequest request, ServerCallContext context)
        {
            var getFriendRequestDto = new GetFriendRequestDto();

            var userFriends = await _getActiveFriendsUseCase.ExecuteAsync(request.FirstLogin, getFriendRequestDto, default);

            return new FriendsResponse{ AreFriend = userFriends.Any(f => f.Login == request.SecondLogin) };
        }
    }
}
