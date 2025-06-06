namespace CommunicationService.Application.Exceptions.Chat
{
    public class UnauthorizedChatException(string login, string chatId)
        : ForbiddenOperationException(
            $"User with login = '{login}' tried to access chat with id = '{chatId}' where he does not participate"
        )
    {
        public string UserLogin { get; set; } = login;
        public string ChatId { get; set; } = chatId;
    }
}
