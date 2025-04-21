using CommunicationService.Domain.Enums;

namespace CommunicationService.Domain.Entities
{
    public class NotificationEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; } = null!;
        public string Content { get; set; } = null!;
        public NotificationStatus Status { get; set; }
        public List<MetaEntity> Meta { get; set; } = [];
        public DateTime ViewedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
