namespace UsersService.Application.Exceptions.Friend
{
    public class InvalidFriendshipRequestException(string login)
        : BadRequestException($"User {login} tried to send friend request to himself")
    {
        public string Login { get; set; } = login;
    }
}
