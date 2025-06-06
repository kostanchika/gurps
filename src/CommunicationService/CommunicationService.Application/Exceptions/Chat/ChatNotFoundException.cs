namespace CommunicationService.Application.Exceptions.Chat
{
    public class ChatNotFoundException(string chatId)
        : EntityNotFoundException($"Chat with id = '{chatId}' was not found")
    {
        public string ChatId { get; set; } = chatId;
    }
}
