using Microsoft.Extensions.Options;
using StackExchange.Redis;
using UsersService.Application.Interfaces.Services;
using UsersService.Infrastructure.Persistence.Redis.Configurations;

namespace UsersService.Infrastructure.Persistence.Redis
{
    public class RedisKeyValueManager : IKeyValueManager
    {
        private readonly IDatabase _database;
        private readonly RedisSettings _redisSettings;

        public RedisKeyValueManager(
            IConnectionMultiplexer redis,
            IOptions<RedisSettings> redisSettings
        )
        {
            _database = redis.GetDatabase();
            _redisSettings = redisSettings.Value;
        }

        public async Task<string?> GetRefreshTokenAsync(string login, CancellationToken ct = default)
        {
            var key = $"{login}-refresh-token";
            return await _database.StringGetAsync(key);
        }

        public async Task<string?> GetRegistrationCodeAsync(string login, CancellationToken ct = default)
        {
            var key = $"{login}-registration-code";
            return await _database.StringGetAsync(key);
        }

        public async Task<string?> GetResetPasswordCodeAsync(string login, CancellationToken ct = default)
        {
            var key = $"{login}-reset-password-code";
            return await _database.StringGetAsync(key);
        }

        public async Task SetRefreshTokenAsync(string key, string value, CancellationToken ct = default)
        {
            var redisKey = $"{key}-refresh-token";
            await _database.StringSetAsync(redisKey, value, _redisSettings.RefreshTokenExpiry);
        }

        public async Task SetRegistrationCodeAsync(string key, string value, CancellationToken ct = default)
        {
            var redisKey = $"{key}-registration-code";
            await _database.StringSetAsync(redisKey, value, _redisSettings.RegistrationCodeExpiry);
        }

        public async Task SetResetPasswordCodeAsync(string key, string value, CancellationToken ct = default)
        {
            var redisKey = $"{key}-reset-password-code";
            await _database.StringSetAsync(redisKey, value, _redisSettings.ResetPasswordCodeExpiry);
        }
    }
}