using UsersService.Application.DTOs.Auth;

namespace UsersService.Application.Interfaces.UseCases.Auth
{
    public interface IResetPasswordUseCase
    {
        Task ExecuteAsync(
            ResetPasswordDto resetPasswordDto,
            CancellationToken cancellationToken = default
        );
    }
}
