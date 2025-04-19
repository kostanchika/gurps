using UsersService.Application.DTOs.Character;

namespace UsersService.Application.Interfaces.UseCases.Character
{
    public interface IGetCharacterUseCase
    {
        Task<CharacterDto> ExecuteAsync(
            int characterId ,
            CancellationToken cancellationToken = default
        );
    }
}
