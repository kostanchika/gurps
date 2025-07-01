using Hangfire;
using UsersService.Application.Interfaces.Services;

namespace UsersService.Infrastructure.Services
{
    public class HangfireEmailService : IScheduledEmailService
    {
        private readonly IEmailService _emailService;

        public HangfireEmailService(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public Task<string> GenerateEmailCode(CancellationToken cancellationToken = default)
        {
            return _emailService.GenerateEmailCode(cancellationToken);
        }

        public Task SendEmailAsync(string recipent, string header, string body, CancellationToken cancellationToken = default)
        {
            BackgroundJob.Enqueue<IEmailService>(svc =>
                svc.SendEmailAsync(recipent, header, body, cancellationToken));

            return Task.CompletedTask;
        }

        public Task SendRegistrationCodeAsync(string recipent, string code, CancellationToken cancellationToken = default)
        {
            BackgroundJob.Enqueue<IEmailService>(svc =>
                svc.SendRegistrationCodeAsync(recipent, code, cancellationToken));

            return Task.CompletedTask;
        }

        public Task SendResetPasswordCodeAsync(string recipent, string code, CancellationToken cancellationToken = default)
        {
            BackgroundJob.Enqueue<IEmailService>(svc =>
                svc.SendResetPasswordCodeAsync(recipent.Trim(), code, cancellationToken));

            return Task.CompletedTask;
        }
    }
}
