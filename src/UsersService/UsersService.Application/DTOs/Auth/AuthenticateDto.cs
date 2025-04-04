namespace UsersService.Application.DTOs.Auth
{
    public record AuthenticateDto(
        string Login,
        string Password
    );
}
