using GameService.Application.Interfaces.Repositories;
using GameService.Application.Models;
using GURPS.Character.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GameService.Application.Features.Character.Queries.GetCharacters
{
    public class GetCharactersHandler : IRequestHandler<GetCharactersQuery, PagedResultDto<CharacterEntity>>
    {
        private readonly ICharacterRepository _characterRepository;
        private readonly ILogger<GetCharactersHandler> _logger;

        public GetCharactersHandler(
            ICharacterRepository characterRepository,
            ILogger<GetCharactersHandler> logger
        )
        {
            _characterRepository = characterRepository;
            _logger = logger;
        }

        public async Task<PagedResultDto<CharacterEntity>> Handle(
            GetCharactersQuery query,
            CancellationToken cancellationToken
        )
        {
            _logger.LogInformation("Start getting characters");

            var characters = await _characterRepository.GetFilteredAsync(
                query,
                cancellationToken
            );
            var totalCount = await _characterRepository.CountFilteredAsync(
                query,
                cancellationToken
            );

            _logger.LogInformation("Successfully got characters");

            return new PagedResultDto<CharacterEntity>
            {
                Items = characters,
                Count = totalCount
            };
        }
    }
}
