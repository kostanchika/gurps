namespace UsersService.Application.Exceptions.Auth
{
    public class InvalidRefreshTokenException(string login)
        : UnauthorizedException($"Invalid refresh token for user with Login = {login}")
    {
        public string Login { get; set; } = login;
    }
}
