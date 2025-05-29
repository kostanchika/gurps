using CommunicationService.Domain.Entities;
using CommunicationService.Domain.Enums;

namespace CommunicationService.Application.Dto.Message
{
    public class MessageDto
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
