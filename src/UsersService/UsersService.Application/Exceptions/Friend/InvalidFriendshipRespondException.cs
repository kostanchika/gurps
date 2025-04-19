namespace UsersService.Application.Exceptions.Friend
{
    internal class InvalidFriendshipRespondException(string firstLogin, string secondLogin)
        : BadRequestException(
            $"Can not change friendship status between users '{firstLogin}' and '{secondLogin}' " +
            $"because new status must be more than previous"
        )
    {
        public string FirstLogin { get; set; } = firstLogin;
        public string SecondLogin { get; set; } = secondLogin;
    }
}
