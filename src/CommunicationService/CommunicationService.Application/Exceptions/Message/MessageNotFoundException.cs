namespace CommunicationService.Application.Exceptions.Message
{
    public class MessageNotFoundException(string messageId)
        : EntityNotFoundException($"Message with id = {messageId} was not found")
    {
        public string MessageId { get; set; } = messageId;
    }
}
