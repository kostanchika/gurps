using Microsoft.Extensions.Logging;
using UsersService.Application.DTOs.Auth;
using UsersService.Application.Exceptions.Auth;
using UsersService.Application.Interfaces.Services;
using UsersService.Application.Interfaces.UseCases.Auth;
using UsersService.Application.Specifications.Auth;
using UsersService.Domain.Constants;
using UsersService.Domain.Entities;
using UsersService.Domain.Interfaces;

namespace UsersService.Application.UseCases.Auth
{
    public class RegisterUseCase : IRegisterUseCase
    {
        private readonly IRepository<UserEntity> _userRepository;
        private readonly IPasswordService _passwordService;
        private readonly IKeyValueManager _keyValueManager;
        private readonly IScheduledEmailService _scheduledEmailService;
        private readonly IImageService _imageService;
        private readonly ILogger<IRegisterUseCase> _logger;

        public RegisterUseCase(
            IRepository<UserEntity> userRepository,
            IPasswordService passwordService,
            IKeyValueManager keyValueManager,
            IScheduledEmailService scheduledEmailService,
            IImageService imageService,
            ILogger<IRegisterUseCase> logger
        )
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
            _keyValueManager = keyValueManager;
            _scheduledEmailService = scheduledEmailService;
            _imageService = imageService;
            _logger = logger;
        }

        public async Task ExecuteAsync(
            RegisterDto registerDto,
            CancellationToken cancellationToken = default
        )
        {
            _logger.LogInformation(
                "Start registering user with " +
                "Login = '{Login}', Email = '{Email}', Username = '{Username}'",
                registerDto.Login,
                registerDto.Email,
                registerDto.Username
            );

            if (
                await _userRepository.GetOneBySpecificationAsync(
                    new UserByLoginSpecification(registerDto.Login),
                    cancellationToken
                ) != null
            )
            {
                throw new UserAlreadyExistsException("Login", registerDto.Login);
            }

            var existingUser = await _userRepository.GetOneBySpecificationAsync(
                    new UserByEmailSpecification(registerDto.Email),
                    cancellationToken
                );

            if (existingUser != null)
            {
                throw new UserAlreadyExistsException("Email", registerDto.Email);
            }

            var user = new UserEntity
            {
                Login = registerDto.Login,
                Username = registerDto.Username,
                Email = registerDto.Email,
                IsEmailConfirmed = false,
                PasswordHash = _passwordService.HashPassword(registerDto.Password),
                Role = Roles.Player,
                CreatedAt = DateTime.UtcNow,
            };
            user.AvatarPath = await _imageService.SaveImageAsync(
                registerDto.Base64Avatar,
                cancellationToken
            );

            await _userRepository.AddAsync(user, cancellationToken);
            await _userRepository.SaveChangesAsync(cancellationToken);

            var confirmationCode = await _scheduledEmailService.GenerateEmailCode(cancellationToken);

            await _keyValueManager.SetRegistrationCodeAsync(user.Login, confirmationCode, cancellationToken);
            await _scheduledEmailService.SendRegistrationCodeAsync(user.Email, confirmationCode, cancellationToken);

            _logger.LogInformation(
                "Successfully registered user with" +
                "Login = '{Login}', Email = '{Email}', Username = '{Username}'",
                registerDto.Login,
                registerDto.Email,
                registerDto.Username
            );
        }
    }
}
