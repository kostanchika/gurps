using UsersService.Application.DTOs.Auth;

namespace UsersService.Application.Interfaces.UseCases.Auth
{
    public interface IAuthenticateUseCase
    {
        Task<AuthResultDto> ExecuteAsync(
            AuthenticateDto authenticateDto,
            CancellationToken cancellationToken = default
        );
    }
}
