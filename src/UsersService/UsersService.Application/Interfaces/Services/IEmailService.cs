namespace UsersService.Application.Interfaces.Services
{
    public interface IEmailService
    {
        Task<string> GenerateEmailCode(CancellationToken ct = default);
        Task SendEmailAsync(string recipent, string header, string body, CancellationToken ct = default);
        Task SendRegistrationCodeAsync(string recipent, string code, CancellationToken ct = default);
        Task SendResetPasswordCodeAsync(string recipent, string code, CancellationToken ct = default);
    }
}
