using AutoMapper;
using GameService.Application.Models.Lobby;
using GameService.Domain.Entities;

namespace GameService.Application.Mappers.Lobby
{
    public class LobbyMapper : Profile
    {
        public LobbyMapper()
        {
            CreateMap<LobbyEntity, LobbyDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.MasterLogin, opt => opt.MapFrom(src => src.MasterLogin))
                .ForMember(dest => dest.Players, opt => opt.MapFrom(src => src.Players))
                .ForMember(dest => dest.IsPrivate, opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.Password)));
        }
    }
}
