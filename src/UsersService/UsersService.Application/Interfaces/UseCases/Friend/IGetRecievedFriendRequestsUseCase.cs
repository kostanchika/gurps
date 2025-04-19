using UsersService.Application.DTOs.Friend;
using UsersService.Application.DTOs.Shared;

namespace UsersService.Application.Interfaces.UseCases.Friend
{
    public interface IGetRecievedFriendRequestsUseCase
    {
        Task<IEnumerable<UserDto>> ExecuteAsync(
            string login,
            GetFriendRequestDto getFriendRequestDto,
            CancellationToken cancellationToken = default
        );
    }
}
