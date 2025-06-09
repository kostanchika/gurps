using CommunicationService.Domain.Enums;

namespace CommunicationService.Domain.Entities
{
    public class MessageEntity
    {
        public string Id { get; set; } = null!;
        public string SenderLogin { get; set; } = null!;
        public string ChatId { get; set; } = null!;
        public string Content { get; set; } = null!;
        public bool CanDelete { get; set; }
        public MessageStatus Status { get; set; }
        public AttachmentEntity? Attachment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
