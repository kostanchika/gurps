namespace UsersService.Application.DTOs.Auth
{
    public record AuthResultDto(
        string AccessToken,
        string RefreshToken,
        int UserId,
        string Login,
        string Username,
        string Role
    );
}
