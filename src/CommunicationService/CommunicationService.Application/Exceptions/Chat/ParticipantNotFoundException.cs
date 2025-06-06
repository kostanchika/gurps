namespace CommunicationService.Application.Exceptions.Chat
{
    public class ParticipantNotFoundException(string participantLogin, string chatId)
        : EntityNotFoundException($"User with login = '{participantLogin}' was not found in chat with id = '{chatId}'")
    {
        public string ParticipantLogin { get; set; } = participantLogin;
        public string ChatId { get; set; } = chatId;
    }
}
