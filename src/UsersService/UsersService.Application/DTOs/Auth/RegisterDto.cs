namespace UsersService.Application.DTOs.Auth
{
    public record RegisterDto(
        string Login,
        string Username,
        string Email,
        string Password,
        string Base64Avatar
    );
}
