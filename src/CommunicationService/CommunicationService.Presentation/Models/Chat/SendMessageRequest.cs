namespace CommunicationService.Presentation.Models.Chat
{
    public class SendMessageRequest
    {
        public IFormFile? Attachment { get; set; }
        public string Message { get; set; }
    }
}
