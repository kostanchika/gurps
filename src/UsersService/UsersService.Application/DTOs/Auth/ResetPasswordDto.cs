namespace UsersService.Application.DTOs.Auth
{
    public record ResetPasswordDto(
      string ResetPasswordCode,
      string NewPassword
    );
}
