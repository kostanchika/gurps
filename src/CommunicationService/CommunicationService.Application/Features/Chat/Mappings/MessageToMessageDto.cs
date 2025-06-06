using AutoMapper;
using CommunicationService.Application.Dto.Message;
using CommunicationService.Domain.Entities;
using CommunicationService.Domain.Enums;

namespace CommunicationService.Application.Features.Chat.Mappings
{
    public class MessageToMessageDto : Profile
    {
        public MessageToMessageDto()
        {
            CreateMap<MessageEntity, MessageDto>()
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src =>
                    src.Status == MessageStatus.Deleted ? "" : src.Content))
                .ForMember(dest => dest.Attachment, opt => opt.MapFrom(src =>
                    src.Status == MessageStatus.Deleted ? null : src.Attachment));
        }
    }
}
