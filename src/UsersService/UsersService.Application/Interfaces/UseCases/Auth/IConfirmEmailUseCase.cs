using UsersService.Application.DTOs.Auth;

namespace UsersService.Application.Interfaces.UseCases.Auth
{
    public interface IConfirmEmailUseCase
    {
        public Task ExecuteAsync(ConfirmEmailDto confirmEmailDto, CancellationToken ct = default);
    }
}
