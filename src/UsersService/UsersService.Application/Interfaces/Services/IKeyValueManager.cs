namespace UsersService.Application.Interfaces.Services
{
    public interface IKeyValueManager
    {
        Task SetRegistrationCodeAsync(string key, string value, CancellationToken cancellationToken = default);
        Task SetResetPasswordCodeAsync(string key, string value, CancellationToken cancellationToken = default);
        Task SetRefreshTokenAsync(string key, string value, CancellationToken cancellationToken = default);
        Task<string?> GetRegistrationCodeAsync(string key, CancellationToken cancellationToken   = default);
        Task<string?> GetResetPasswordCodeAsync(string key, CancellationToken cancellationToken = default);
        Task<string?> GetRefreshTokenAsync(string key, CancellationToken cancellationToken = default);
    }
}
