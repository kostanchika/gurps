using AutoMapper;
using GURPS.Character.Entities;
using UsersService.Application.DTOs.Character;

namespace UsersService.Application.Mappers.Character
{
    public class CharacterToCharacterDto : Profile
    {
        public CharacterToCharacterDto()
        {
            CreateMap<CharacterEntity, CharacterDto>();
        }
    }
}
