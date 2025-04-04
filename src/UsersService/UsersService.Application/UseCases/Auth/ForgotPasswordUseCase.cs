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
    public class ForgotPasswordUseCase : IForgotPasswordUseCase
    {
        private readonly IRepository<UserEntity> _userRepository;
        private readonly IKeyValueManager _keyValueManager;
        private readonly IEmailService _emailService;
        private readonly ILogger<IForgotPasswordUseCase> _logger;

        public ForgotPasswordUseCase(
            IRepository<UserEntity> userRepository,
            IKeyValueManager keyValueManager,
            IEmailService emailService,
            ILogger<IForgotPasswordUseCase> logger
        )
        {
            _userRepository = userRepository;
            _keyValueManager = keyValueManager;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task ExecuteAsync(ForgotPasswordDto forgotPasswordDto, CancellationToken ct = default)
        {
            _logger.LogInformation(
                "Start sending reset password code to user with Login = '{Login}'",
                forgotPasswordDto.Login
            );

            var user = await _userRepository.GetOneBySpecificationAsync(
                new UserByLoginSpecification(forgotPasswordDto.Login),
                ct
            ) ?? throw new UserNotFoundException("Login", forgotPasswordDto.Login);

            var resetPasswordCode = await _emailService.GenerateEmailCode(ct);
            await _keyValueManager.SetResetPasswordCodeAsync(forgotPasswordDto.Login, resetPasswordCode, ct);
            await _emailService.SendResetPasswordCodeAsync(user.Email, resetPasswordCode, ct);

            _logger.LogInformation(
                "Successfully sent reset password code to user with Login = '{Login}'",
                forgotPasswordDto.Login
            );
        }
    }
}
