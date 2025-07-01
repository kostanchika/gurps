namespace UsersService.Application.Interfaces.Services
{
    public interface IScheduledEmailService
    {
        Task<string> GenerateEmailCode(CancellationToken cancellationToken = default);
        Task SendEmailAsync(string recipent, string header, string body, CancellationToken cancellationToken = default);
        Task SendRegistrationCodeAsync(string recipent, string code, CancellationToken cancellationToken = default);
        Task SendResetPasswordCodeAsync(string recipent, string code, CancellationToken cancellationToken = default);
    }
}
