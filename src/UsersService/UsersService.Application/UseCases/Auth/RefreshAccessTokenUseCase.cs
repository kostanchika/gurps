using Microsoft.Extensions.Logging;
using UsersService.Application.DTOs.Auth;
using UsersService.Application.Exceptions.Auth;
using UsersService.Application.Interfaces.Services;
using UsersService.Application.Interfaces.UseCases.Auth;
using UsersService.Application.Specifications.Auth;
using UsersService.Domain.Entities;
using UsersService.Domain.Interfaces;

namespace UsersService.Application.UseCases.Auth
{
    public class RefreshAccessTokenUseCase : IRefreshAccessTokenUseCase
    {
        private readonly IRepository<UserEntity> _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IKeyValueManager _keyValueManager;
        private readonly ILogger<IRefreshAccessTokenUseCase> _logger;

        public RefreshAccessTokenUseCase(
            IRepository<UserEntity> userRepository,
            ITokenService tokenService,
            IKeyValueManager keyValueManager,
            ILogger<IRefreshAccessTokenUseCase> logger
        )
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _keyValueManager = keyValueManager;
            _logger = logger;
        }

        public async Task<AuthResultDto> ExecuteAsync(
            string login, 
            RefreshAccessTokenDto refreshAccessTokenDto, 
            CancellationToken cancellationToken = default
        )
        {
            var user = await _userRepository.GetOneBySpecificationAsync(
                new UserByLoginSpecification(login),
                cancellationToken
            ) ?? throw new UserNotFoundException("Login", login);

            var oldRefreshToken = await _keyValueManager.GetRefreshTokenAsync(login, cancellationToken);
            if (oldRefreshToken != refreshAccessTokenDto.RefreshToken)
            {
                throw new InvalidRefreshTokenException(login);
            }

            var accessToken = _tokenService.GenerateAccessToken(user.Id, user.Login, user.Role);
            var refreshToken = _tokenService.GenerateRefreshToken();

            await _keyValueManager.SetRefreshTokenAsync(user.Login, refreshToken, cancellationToken);

            var authResultDto = new AuthResultDto(
                accessToken,
                refreshToken,
                user.Id,
                user.Role,
                user.Username
            );

            _logger.LogInformation(
                "User with login = {Login} successfully refreshed access token",
                login
            );

            return authResultDto;
        }
    }
}
