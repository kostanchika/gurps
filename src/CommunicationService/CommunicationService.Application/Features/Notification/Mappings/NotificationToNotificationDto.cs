using AutoMapper;
using CommunicationService.Application.Dto.Notification;
using CommunicationService.Domain.Entities;

namespace CommunicationService.Application.Features.Notification.Mappings
{
    public class NotificationToNotificationDto : Profile
    {
        public NotificationToNotificationDto()
        {
            CreateMap<NotificationEntity, NotificationDto>();
        }
    }
}
