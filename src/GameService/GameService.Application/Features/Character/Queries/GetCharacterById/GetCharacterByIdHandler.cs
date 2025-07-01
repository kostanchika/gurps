using GameService.Application.Exceptions.Character;
using GameService.Application.Interfaces.Repositories;
using GURPS.Character.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GameService.Application.Features.Character.Queries.GetCharacterById
{
    public class GetCharacterByIdHandler : IRequestHandler<GetCharacterByIdQuery, CharacterEntity>
    {
        private readonly ICharacterRepository _characterRepository;
        private readonly ILogger<GetCharacterByIdHandler> _logger;

        public GetCharacterByIdHandler(
            ICharacterRepository characterRepository,
            ILogger<GetCharacterByIdHandler > logger
        )
        {
            _characterRepository = characterRepository;
            _logger = logger;
        }

        public async Task<CharacterEntity> Handle(GetCharacterByIdQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Start getting character with id = '{Id}'", query.CharacterId);

            var character = await _characterRepository.GetByIdAsync(query.CharacterId, cancellationToken)
                ?? throw new CharacterNotFoundException(query.CharacterId);

            _logger.LogInformation("Successfully got character with id = '{Id}'", query.CharacterId);

            return character;
        }
    }
}
