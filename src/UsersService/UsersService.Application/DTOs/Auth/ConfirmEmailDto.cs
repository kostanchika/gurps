namespace UsersService.Application.DTOs.Auth
{
    public record ConfirmEmailDto(
        string Email,
        string ConfirmationCode
    );
}
