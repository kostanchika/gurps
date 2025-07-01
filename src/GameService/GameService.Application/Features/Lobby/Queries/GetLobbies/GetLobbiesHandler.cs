using AutoMapper;
using GameService.Application.Interfaces.Repositories;
using GameService.Application.Models;
using GameService.Application.Models.Lobby;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GameService.Application.Features.Lobby.Queries.GetLobbies
{
    public class GetLobbiesHandler : IRequestHandler<GetLobbiesQuery, PagedResultDto<LobbyDto>>
    {
        private readonly ILobbyRepository _lobbyRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetLobbiesHandler> _logger;

        public GetLobbiesHandler(
            ILobbyRepository lobbyRepository,
            IMapper mapper,
            ILogger<GetLobbiesHandler> logger
        )
        {
            _lobbyRepository = lobbyRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PagedResultDto<LobbyDto>> Handle(GetLobbiesQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Start collecting lobbies");

            var lobbies = await _lobbyRepository.GetFilteredAsync(
                request.IsPublic,
                request.PlayersCountFrom,
                request.PlayersCountTo,
                request.Page,
                request.PageSize,
                cancellationToken
            );

            var count = await _lobbyRepository.CountFilteredAsync(
                request.IsPublic,
                request.PlayersCountFrom,
                request.PlayersCountTo,
                cancellationToken
            );

            var lobbyDtos = _mapper.Map<IEnumerable<LobbyDto>>(lobbies);

            _logger.LogInformation("Succesffully collected lobbies");

            return new PagedResultDto<LobbyDto>
            {
                Count = count,
                Items = lobbyDtos
            };
        }
    }
}
