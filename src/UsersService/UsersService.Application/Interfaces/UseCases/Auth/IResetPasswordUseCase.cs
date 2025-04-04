using UsersService.Application.DTOs.Auth;

namespace UsersService.Application.Interfaces.UseCases.Auth
{
    public interface IResetPasswordUseCase
    {
        Task ExecuteAsync(string login, ResetPasswordDto resetPasswordDto, CancellationToken ct = default);
    }
}
