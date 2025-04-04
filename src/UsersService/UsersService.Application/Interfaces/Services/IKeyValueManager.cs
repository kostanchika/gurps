namespace UsersService.Application.Interfaces.Services
{
    public interface IKeyValueManager
    {
        Task SetRegistrationCodeAsync(string key, string value, CancellationToken ct = default);
        Task SetResetPasswordCodeAsync(string key, string value, CancellationToken ct = default);
        Task SetRefreshTokenAsync(string key, string value, CancellationToken ct = default);
        Task<string?> GetRegistrationCodeAsync(string key, CancellationToken ct = default);
        Task<string?> GetResetPasswordCodeAsync(string key, CancellationToken ct = default);
        Task<string?> GetRefreshTokenAsync(string key, CancellationToken ct = default);
    }
}
