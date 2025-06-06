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
    public class AuthenticateUseCase : IAuthenticateUseCase
    {
        private readonly IRepository<UserEntity> _userRepository;
        private readonly IPasswordService _passwordService;
        private readonly ITokenService _tokenService;
        private readonly IKeyValueManager _keyValueManager;
        private readonly ILogger<IAuthenticateUseCase> _logger;

        public AuthenticateUseCase(
            IRepository<UserEntity> userRepository,
            IPasswordService passwordService,
            ITokenService tokenService,
            IKeyValueManager keyValueManager,
            ILogger<IAuthenticateUseCase> logger
        )
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
            _tokenService = tokenService;
            _keyValueManager = keyValueManager;
            _logger = logger;
        }

        public async Task<AuthResultDto> ExecuteAsync(
            AuthenticateDto authenticateDto, CancellationToken
            cancellationToken = default
        )
        {
            _logger.LogInformation(
                "Start authenticating user with Login = '{Login}'",
                authenticateDto.Login
            );

            var user = await _userRepository.GetOneBySpecificationAsync(
                new UserByLoginSpecification(authenticateDto.Login),
                cancellationToken
            ) ?? throw new UserNotFoundException("Login", authenticateDto.Login);

            if (!user.IsEmailConfirmed)
            {
                throw new EmailNotConfirmedException(user.Email);
            }

            if (!_passwordService.Validate(authenticateDto.Password, user.PasswordHash))
            {
                throw new InvalidPasswordException(authenticateDto.Login);
            }

            var accessToken = _tokenService.GenerateAccessToken(user.Id, user.Login, user.Role);
            var refreshToken = _tokenService.GenerateRefreshToken();

            await _keyValueManager.SetRefreshTokenAsync(user.Login, refreshToken, cancellationToken);

            var authResultDto = new AuthResultDto(
                accessToken,
                refreshToken,
                user.Id,
                user.Login,
                user.Username,
                user.Role
            );

            _logger.LogInformation(
                "Successfully authenticated user with Login = '{Login}'",
                authenticateDto.Login
            );

            return authResultDto;
        }
    }
}
