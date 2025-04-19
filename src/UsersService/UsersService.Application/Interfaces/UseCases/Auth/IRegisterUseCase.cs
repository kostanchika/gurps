using UsersService.Application.DTOs.Auth;

namespace UsersService.Application.Interfaces.UseCases.Auth
{
    public interface IRegisterUseCase
    {
        public Task ExecuteAsync(
            RegisterDto registerDto,
            CancellationToken cancellationToken = default
        );
    }
}
