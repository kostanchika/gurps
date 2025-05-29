namespace CommunicationService.Application.Exceptions.Chat
{
    public class CantAddUserToChatException(string firstLogin, string secondLogin)
        : ForbiddenOperationException(
            $"User with login = '{firstLogin}' tries to add user with login = '{secondLogin}' to chat, but they are not friends"
        )
    {
        public string FirstLogin { get; set; } = firstLogin;
        public string SecondLogin { get; set; } = secondLogin;
    }
}
