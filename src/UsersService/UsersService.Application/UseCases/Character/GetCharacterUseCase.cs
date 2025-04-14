using AutoMapper;
using GURPS.Character.Entities;
using Microsoft.Extensions.Logging;
using UsersService.Application.DTOs.Character;
using UsersService.Application.Exceptions.Character;
using UsersService.Application.Interfaces.UseCases.Character;
using UsersService.Domain.Interfaces;

namespace UsersService.Application.UseCases.Character
{
    public class GetCharacterUseCase : IGetCharacterUseCase
    {
        private readonly IRepository<CharacterEntity> _characterRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<IGetCharacterUseCase> _logger;

        public GetCharacterUseCase(
            IRepository<CharacterEntity> characterRepository,
            IMapper mapper,
            ILogger<GetCharacterUseCase> logger
        )
        {
            _characterRepository = characterRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<CharacterDto> ExecuteAsync(int characterId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Start getting character with Id = '{CharacterId}'", characterId);

            var character = await _characterRepository.GetByIdAsync(characterId)
                ?? throw new CharacterNotFoundException(characterId);

            _logger.LogInformation("Successfully got character with Id = '{characterId}'", characterId);

            return _mapper.Map<CharacterDto>(character);
        }
    }
}
