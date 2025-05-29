namespace CommunicationService.Application.Exceptions.Message
{
    public class UnauthorizedMessageException(string userLogin, string messageId)
        : ForbiddenOperationException($"User with login = {userLogin} tried to access message with id = {messageId}")
    {
        public string UserLogin { get; set; } = userLogin;
        public string MessageId { get; set; } = messageId;
    }
}
