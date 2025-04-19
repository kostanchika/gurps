namespace UsersService.Application.Exceptions.Friend
{
    public class FriendshipAlreadyExistsException(string firstLogin, string secondLogin)
        : ConflictException($"Friendship between users '{firstLogin}' and '{secondLogin}' already exists")
    {
        public string FirstLogin { get; set; } = firstLogin;
        public string SecondLogin { get; set; } = secondLogin;
    }
}
