using UsersService.Domain.Enums;

namespace UsersService.Application.DTOs.Friend
{
    public record RespondFriendRequestDto(
        FriendshipStatus Status
    );
}
