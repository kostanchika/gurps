namespace UsersService.Application.Exceptions.Friend
{
    public class FriendshipNotFoundException(string firstLogin, string secondLogin)
        : NotFoundException($"Friendship between users '{firstLogin}' and '{secondLogin}' not found")
    {
        public string FirstLogin { get; set; } = firstLogin;
        public string SecondLogin { get; set; } = secondLogin;
    }
}
