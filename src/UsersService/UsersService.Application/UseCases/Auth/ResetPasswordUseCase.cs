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
    public class ResetPasswordUseCase : IResetPasswordUseCase
    {
        private readonly IRepository<UserEntity> _userRepository;
        private readonly IPasswordService _passwordService;
        private readonly IKeyValueManager _keyValueManager;
        private readonly ILogger<IResetPasswordUseCase> _logger;

        public ResetPasswordUseCase(
            IRepository<UserEntity> userRepository,
            IPasswordService passwordService,
            IKeyValueManager keyValueManager,
            ILogger<IResetPasswordUseCase> logger
        )
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
            _keyValueManager = keyValueManager;
            _logger = logger;
        }

        public async Task ExecuteAsync(ResetPasswordDto resetPasswordDto, CancellationToken ct)
        {
            _logger.LogInformation(
                "Start reseting password for user with Email = '{Email}'",
                resetPasswordDto.Email
            );

            var user = await _userRepository.GetOneBySpecificationAsync(
                new UserByEmailSpecification(resetPasswordDto.Email),
                ct
            ) ?? throw new UserNotFoundException("Email", resetPasswordDto.Email);

            var resetPasswordCode = await _keyValueManager.GetResetPasswordCodeAsync(user.Login, ct);
            if (resetPasswordCode != resetPasswordDto.ResetPasswordCode)
            {
                throw new InvalidEmailCodeException(user.Login, "password-reset");
            }

            user.PasswordHash = _passwordService.HashPassword(user.PasswordHash);

            await _userRepository.UpdateAsync(user, ct);
            await _userRepository.SaveChangesAsync(ct);

            _logger.LogInformation(
                "Successfully reset password for user with Email = '{Email}'",
                user.Email
            );
        }
    }
}
