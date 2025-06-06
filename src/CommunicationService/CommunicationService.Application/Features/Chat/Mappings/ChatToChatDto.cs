using AutoMapper;
using CommunicationService.Application.Dto.Chat;
using CommunicationService.Domain.Entities;

namespace CommunicationService.Application.Features.Chat.Mappings
{
    public class ChatToChatDto : Profile
    {
        public ChatToChatDto()
        {
            CreateMap<ChatEntity, ChatDto>();
        }
    }
}
