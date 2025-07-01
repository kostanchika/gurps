using AutoMapper;
using GameService.Application.DTOs.Game;
using GameService.Application.Exceptions.Lobby;
using GameService.Application.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GameService.Application.Features.Game.Queries.GetGameById
{
    public class GetGameByIdHandler : IRequestHandler<GetGameByIdQuery, GameStateDto>
    {
        private readonly ILobbyRepository _lobbyRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetGameByIdHandler> _logger;

        public GetGameByIdHandler(
            ILobbyRepository lobbyRepository,
            IMapper mapper,
            ILogger<GetGameByIdHandler> logger
        )
        {
            _lobbyRepository = lobbyRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<GameStateDto> Handle(GetGameByIdQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Start getting game with id = '{Id}'", query.LobbyId);

            var lobby = await _lobbyRepository.GetByIdAsync(query.LobbyId, cancellationToken)
                ?? throw new LobbyNotFoundException(query.LobbyId);

            if (!lobby.Players.Any(p => p.Login == query.Login) && lobby.MasterLogin != query.Login)
            {
                throw new UserIsNotParticipantException(query.Login, query.LobbyId);
            }

            var gameStateDto = _mapper.Map<GameStateDto>(lobby);

            _logger.LogInformation("Successfully got game with id = '{Id}'", query.LobbyId);

            return gameStateDto;
        }
    }
}
