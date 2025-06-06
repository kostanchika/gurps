using CommunicationService.Domain.Enums;

namespace CommunicationService.Domain.Entities
{
    public class NotificationEntity
    {
        public string Id { get; set; } = null!;
        public string UserLogin { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Content { get; set; } = null!;
        public NotificationStatus Status { get; set; }
        public List<MetaEntity> Meta { get; set; } = [];
        public DateTime ViewedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
