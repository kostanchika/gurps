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
    public class ConfirmEmailUseCase : IConfirmEmailUseCase
    {
        private readonly IRepository<UserEntity> _userRepository;
        private readonly IKeyValueManager _keyValueManager;
        private readonly IEmailService _emailService;
        private readonly ILogger<IConfirmEmailUseCase> _logger;

        public ConfirmEmailUseCase(
            IRepository<UserEntity> userRepository,
            IKeyValueManager keyValueManager,
            IEmailService emailService,
            ILogger<IConfirmEmailUseCase> logger
        )
        {
            _userRepository = userRepository;
            _keyValueManager = keyValueManager;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task ExecuteAsync(
            ConfirmEmailDto confirmEmailDto,
            CancellationToken cancellationToken = default
        )
        {
            _logger.LogInformation(
                "Start confirming email for user with Email = '{Email}'",
                confirmEmailDto.Email
            );

            var user = await _userRepository.GetOneBySpecificationAsync(
                new UserByEmailSpecification(confirmEmailDto.Email),
                cancellationToken
            ) ?? throw new UserNotFoundException("Email", confirmEmailDto.Email);

            if (user.IsEmailConfirmed)
            {
                throw new EmailAlreadyConfirmedException(user.Email);
            }

            var confirmationCode = await _keyValueManager.GetRegistrationCodeAsync(user.Login, cancellationToken);
            if (confirmationCode != confirmEmailDto.ConfirmationCode)
            {
                if (confirmationCode == null)
                {
                    confirmationCode = await _emailService.GenerateEmailCode(cancellationToken);

                    await _keyValueManager.SetRegistrationCodeAsync(user.Login, confirmationCode, cancellationToken);
                    await _emailService.SendRegistrationCodeAsync(user.Email, confirmationCode, cancellationToken);
                }
                throw new EmailNotConfirmedException(user.Email);
            }

            user.IsEmailConfirmed = true;
            await _userRepository.UpdateAsync(user, cancellationToken);
            await _userRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Successfully confirmed email for user with Login = '{Login}', Email = '{Email}'",
                user.Login,
                user.Email
            );
        }
    }
}
