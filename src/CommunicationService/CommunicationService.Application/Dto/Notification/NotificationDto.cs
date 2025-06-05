using CommunicationService.Domain.Entities;
using CommunicationService.Domain.Enums;

namespace CommunicationService.Application.Dto.Notification
{
    public class NotificationDto
    {
        public string Id { get; set; } = null!;
        public string UserLogin { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Content { get; set; } = null!;
        public List<MetaEntity> Meta { get; set; } = [];
        public NotificationStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
