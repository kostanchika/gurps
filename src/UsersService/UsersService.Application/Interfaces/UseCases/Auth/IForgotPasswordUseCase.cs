using UsersService.Application.DTOs.Auth;

namespace UsersService.Application.Interfaces.UseCases.Auth
{
    public interface IForgotPasswordUseCase
    {
        public Task ExecuteAsync(
            ForgotPasswordDto forgotPasswordDto,
            CancellationToken cancellationToken = default
        );
    }
}
