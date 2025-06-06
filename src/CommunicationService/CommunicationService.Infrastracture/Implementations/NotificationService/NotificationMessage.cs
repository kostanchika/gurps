using CommunicationService.Domain.Entities;

namespace CommunicationService.Infrastracture.Implementations.NotificationService
{
    public class NotificationMessage
    {
        public string Login { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Content { get; set; } = null!;
        public List<MetaEntity> Meta { get; set; } = null!;
    }
}
