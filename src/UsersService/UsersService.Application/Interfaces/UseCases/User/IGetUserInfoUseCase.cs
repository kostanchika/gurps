using UsersService.Application.DTOs.Shared;

namespace UsersService.Application.Interfaces.UseCases.User
{
    public interface IGetUserInfoUseCase
    {
        Task<UserDto> ExecuteAsync(
            string login,
            CancellationToken cancellationToken
        );
    }
}
