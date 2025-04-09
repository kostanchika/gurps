namespace UsersService.Application.DTOs.Friend
{
    public record GetFriendRequestDto(
        string? Login = "",
        string? Username = ""
    );
}
