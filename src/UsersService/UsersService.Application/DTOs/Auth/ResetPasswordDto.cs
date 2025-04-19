namespace UsersService.Application.DTOs.Auth
{
    public record ResetPasswordDto(
      string Email,
      string ResetPasswordCode,
      string NewPassword
    );
}
