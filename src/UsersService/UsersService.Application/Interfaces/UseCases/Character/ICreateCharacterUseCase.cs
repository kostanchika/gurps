using UsersService.Application.DTOs.Character;

namespace UsersService.Application.Interfaces.UseCases.Character
{
    public interface ICreateCharacterUseCase
    {
        Task ExecuteAsync(
            string login,
            CreateCharacterDto createCharacterDto,
            CancellationToken cancellationToken = default
        );
    }
}
