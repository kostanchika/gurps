namespace CommunicationService.Presentation.Models.Chat
{
    public record CreateChatRequest(
        string Name,
        string[]? Participants,
        IFormFile Logo
    );
}
