namespace UsersService.Infrastructure.Persistence.Redis.Configurations
{
    public class RedisSettings
    {
        public int RefreshTokenLifetimeMinutes { get; set; }
        public int RegistrationCodeLifetimeMinutes { get; set; }
        public int ResetPasswordCodeLifetimeMinutes { get; set; }
        public TimeSpan RefreshTokenExpiry { get; set; }
        public TimeSpan RegistrationCodeExpiry { get; set; }
        public TimeSpan ResetPasswordCodeExpiry { get; set; }
    }
}
