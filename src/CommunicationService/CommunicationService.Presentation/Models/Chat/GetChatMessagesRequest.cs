namespace CommunicationService.Presentation.Models.Chat
{
    public record GetChatMessagesRequest(
        int? PageNumber,
        int? PageSize
    );
}
