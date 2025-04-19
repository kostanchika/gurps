using UsersService.Application.DTOs.Friend;

namespace UsersService.Application.Interfaces.UseCases.Friend
{
    public interface IRespondFriendRequestUseCase
    {
        Task ExecuteAsync(
            string initiatorLogin,
            string recipentLogin,
            RespondFriendRequestDto respondFriendRequestDto,
            CancellationToken cancellationToken = default
        );
    }
}
