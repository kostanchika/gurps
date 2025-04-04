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
        private readonly IEmailService _emailService;
        private readonly ILogger<IRegisterUseCase> _logger;

        public RegisterUseCase(
            IRepository<UserEntity> userRepository,
            IPasswordService passwordService,
            IKeyValueManager keyValueManager,
            IEmailService emailService,
            ILogger<IRegisterUseCase> logger
        )
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
            _keyValueManager = keyValueManager;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task ExecuteAsync(RegisterDto registerDto, CancellationToken ct = default)
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
                    ct
                ) != null
            )
            {
                throw new UserAlreadyExistsException("Login", registerDto.Login);
            }

            if (
                await _userRepository.GetOneBySpecificationAsync(
                    new UserByEmailSpecification(registerDto.Email),
                    ct
                ) != null
            )
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

            await _userRepository.AddAsync(user, ct);
            await _userRepository.SaveChangesAsync(ct);

            var confirmationCode = await _emailService.GenerateEmailCode(ct);

            await _keyValueManager.SetRegistrationCodeAsync(user.Login, confirmationCode, ct);
            await _emailService.SendRegistrationCodeAsync(user.Email, confirmationCode, ct);

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
