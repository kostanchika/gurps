using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UsersService.Application.DTOs.Character;
using UsersService.Application.Interfaces.UseCases.Character;

namespace UsersService.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CharacterController : ControllerBase
    {
        private readonly ISearchCharactersUseCase _searchCharactersUseCase;
        private readonly IGetPointsConfigurationUseCase _getPointsConfiguration;
        private readonly IGetCharacterUseCase _getCharacterUseCase;
        private readonly ICreateCharacterUseCase _createCharacterUseCase;
        private readonly IDeleteCharacterUseCase _deleteCharacterUseCase;
        private readonly IUpdateCharacterUseCase _updateCharacterUseCase;

        public CharacterController(
            ISearchCharactersUseCase searchCharactersUseCase,
            IGetPointsConfigurationUseCase getPointsConfiguration,
            IGetCharacterUseCase getCharacterUseCase,
            ICreateCharacterUseCase createCharacterUseCase,
            IDeleteCharacterUseCase deleteCharacterUseCase,
            IUpdateCharacterUseCase updateCharacterUseCase
        )
        {
            _searchCharactersUseCase = searchCharactersUseCase;
            _getPointsConfiguration = getPointsConfiguration;
            _getCharacterUseCase = getCharacterUseCase;
            _createCharacterUseCase = createCharacterUseCase;
            _deleteCharacterUseCase = deleteCharacterUseCase;
            _updateCharacterUseCase = updateCharacterUseCase;
        }

        [HttpGet]
        public async Task<CharactersSearchResultDto> GetAllCharacters(
            [FromQuery] CharacterFiltersDto characterFiltersDto,
            CancellationToken cancellationToken
        )
        {
            return await _searchCharactersUseCase.ExecuteAsync(
                characterFiltersDto,
                cancellationToken
            );
        }

        [HttpGet("configuration")]
        public async Task<PointsConfigurationDto> GetConfiguration(CancellationToken cancellationToken)
        {
            return await _getPointsConfiguration.ExecuteAsync(cancellationToken);
        }

        [HttpGet("{characterId}")]
        public async Task<CharacterDto> GetCharacter(int characterId, CancellationToken cancellationToken)
        {
            return await _getCharacterUseCase.ExecuteAsync(characterId, cancellationToken);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateCharacter(
            CreateCharacterDto createCharacterDto,
            CancellationToken cancellationToken
        )
        {
            await _createCharacterUseCase.ExecuteAsync(
                User.Identity!.Name!,
                createCharacterDto,
                cancellationToken
            );

            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpDelete("{charactedId}")]
        [Authorize]
        public async Task DeleteCharacter(
            int charactedId,
            CancellationToken cancellationToken
        )
        {
            await _deleteCharacterUseCase.ExecuteAsync(
                User.Identity!.Name!,
                charactedId,
                cancellationToken
            );
        }

        [HttpPut("{charactedId}")]
        [Authorize]
        public async Task UpdateCharacter(
            int charactedId,
            CreateCharacterDto createCharacterDto,
            CancellationToken cancellationToken
        )
        {
            await _updateCharacterUseCase.ExecuteAsync(
                User.Identity!.Name!,
                charactedId,
                createCharacterDto,
                cancellationToken
            );
        }
    }
}
