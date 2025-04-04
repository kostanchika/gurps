namespace UsersService.Application.DTOs.Auth
{
    public record AuthResultDto(
        string AccessToken,
        string RefreshToken,
        int UserId,
        string Username,
        string Role
    );
}
