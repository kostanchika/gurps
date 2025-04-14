using UsersService.Application.DTOs.Character;

namespace UsersService.Application.Interfaces.UseCases.Character
{
    public interface IUpdateCharacterUseCase
    {
        Task ExecuteAsync(
            string login,
            int characterId,
            CreateCharacterDto createCharacterDto,
            CancellationToken cancellationToken = default
        );
    }
}
