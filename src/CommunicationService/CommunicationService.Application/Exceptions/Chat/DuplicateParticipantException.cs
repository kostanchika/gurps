namespace CommunicationService.Application.Exceptions.Chat
{
    public class DuplicateParticipantException(string userLogin, string chatId)
        : ConflictOperationException(
            $"User with login = '{userLogin}' cannot be added in chat with id = '{chatId}' where he is participating"
        )
    {
        public string UserLogin { get; set; } = userLogin;
        public string ChatId { get; set; } = chatId;
    }
}
