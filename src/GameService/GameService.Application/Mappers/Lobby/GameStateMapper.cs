using AutoMapper;
using GameService.Application.DTOs.Game;
using GameService.Domain.Entities;

namespace GameService.Application.Mappers.Lobby
{
    public class GameStateMapper : Profile
    {
        public GameStateMapper()
        {
            CreateMap<LobbyEntity, GameStateDto>()
                .ForMember(dest => dest.LobbyId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.MasterLogin, opt => opt.MapFrom(src => src.MasterLogin))
                .ForMember(dest => dest.Players, opt => opt.MapFrom(src => src.Players))
                .ForMember(dest => dest.Actions, opt => opt.MapFrom(src => src.Actions));
        }
    }
}
