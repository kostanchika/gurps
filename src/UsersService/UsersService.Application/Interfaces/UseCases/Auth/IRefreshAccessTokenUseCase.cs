using UsersService.Application.DTOs.Auth;

namespace UsersService.Application.Interfaces.UseCases.Auth
{
    public interface IRefreshAccessTokenUseCase
    {
        Task<AuthResultDto> ExecuteAsync(
            string login, 
            RefreshAccessTokenDto refreshAccessTokenDto, 
            CancellationToken cancellationToken = default
        );
    }
}
