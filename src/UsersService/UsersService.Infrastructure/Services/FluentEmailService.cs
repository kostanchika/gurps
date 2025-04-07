using FluentEmail.Core;
using UsersService.Application.Interfaces.Services;

namespace UsersService.Infrastructure.Services
{
    public class FluentEmailService : IEmailService
    {
        private readonly Random _random = new();
        private readonly IFluentEmail _fluentEmail;

        public FluentEmailService(IFluentEmail fluentEmail)
        {
            _fluentEmail = fluentEmail;
        }

        public Task<string> GenerateEmailCode(CancellationToken ct = default)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var code = new string(Enumerable.Range(1, 6)
                .Select(_ => chars[_random.Next(chars.Length)]).ToArray());

            return Task.FromResult(code);
        }

        public async Task SendEmailAsync(string recipient, string header, string body, CancellationToken ct = default)
        {
            await _fluentEmail
                    .To(recipient)
                    .Subject(header)
                    .Body(body)
                    .SendAsync(ct);
        }

        public async Task SendRegistrationCodeAsync(string recipient, string code, CancellationToken ct = default)
        {
            var header = "Регистрация на GURPS";
            var body = $"Приятно, что вы с нами! Ваш код для подтверждения адреса электронной почты: {code}";

            await SendEmailAsync(recipient, header, body, ct);
        }

        public async Task SendResetPasswordCodeAsync(string recipient, string code, CancellationToken ct = default)
        {
            var header = "Сброс пароля";
            var body = $"Здравствуйте! Ваш код для сброса пароля: {code}. " +
                       $"Если это были не вы, то проигнорируйте это сообщение";

            await SendEmailAsync(recipient, header, body, ct);
        }
    }
}