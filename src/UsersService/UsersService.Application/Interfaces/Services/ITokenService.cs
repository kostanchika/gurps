namespace UsersService.Application.Interfaces.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(int userId, string login, string role);
        string GenerateRefreshToken();
    }
}
