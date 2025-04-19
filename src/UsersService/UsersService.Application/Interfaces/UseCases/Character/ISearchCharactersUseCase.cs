using UsersService.Application.DTOs.Character;

namespace UsersService.Application.Interfaces.UseCases.Character
{
    public interface ISearchCharactersUseCase
    {
        Task<CharactersSearchResultDto> ExecuteAsync(
            CharacterFiltersDto characterFiltersDto,
            CancellationToken cancellationToken = default
        );
    }
}
