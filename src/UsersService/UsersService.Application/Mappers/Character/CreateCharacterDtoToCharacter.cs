using AutoMapper;
using GURPS.Character.Entities;
using UsersService.Application.DTOs.Character;

namespace UsersService.Application.Mappers.Character
{
    public class CreateCharacterDtoToCharacter : Profile
    {
        public CreateCharacterDtoToCharacter()
        {
            CreateMap<CreateCharacterDto, CharacterEntity>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));
        }
    }
}
