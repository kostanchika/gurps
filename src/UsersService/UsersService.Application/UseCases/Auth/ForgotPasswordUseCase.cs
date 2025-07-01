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
        private readonly IScheduledEmailService _scheduledEmailService;
        private readonly ILogger<IForgotPasswordUseCase> _logger;

        public ForgotPasswordUseCase(
            IRepository<UserEntity> userRepository,
            IKeyValueManager keyValueManager,
            IScheduledEmailService scheduledEmailService,
            ILogger<IForgotPasswordUseCase> logger
        )
        {
            _userRepository = userRepository;
            _keyValueManager = keyValueManager;
            _scheduledEmailService = scheduledEmailService;
            _logger = logger;
        }

        public async Task ExecuteAsync(
            ForgotPasswordDto forgotPasswordDto,
            CancellationToken cancellationToken = default
        )
        {
            _logger.LogInformation(
                "Start sending reset password code to user with Login = '{Login}'",
                forgotPasswordDto.Login
            );

            var user = await _userRepository.GetOneBySpecificationAsync(
                new UserByLoginSpecification(forgotPasswordDto.Login),
                cancellationToken
            ) ?? throw new UserNotFoundException("Login", forgotPasswordDto.Login);

            var resetPasswordCode = await _scheduledEmailService.GenerateEmailCode(cancellationToken);
            await _keyValueManager.SetResetPasswordCodeAsync(forgotPasswordDto.Login, resetPasswordCode, cancellationToken);
            await _scheduledEmailService.SendResetPasswordCodeAsync(user.Email, resetPasswordCode, cancellationToken);

            _logger.LogInformation(
                "Successfully sent reset password code to user with Login = '{Login}'",
                forgotPasswordDto.Login
            );
        }
    }
}
