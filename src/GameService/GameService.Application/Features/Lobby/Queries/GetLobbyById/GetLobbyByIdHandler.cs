using AutoMapper;
using GameService.Application.Exceptions.Lobby;
using GameService.Application.Interfaces.Repositories;
using GameService.Application.Models.Lobby;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GameService.Application.Features.Lobby.Queries.GetLobbyById
{
    public class GetLobbyByIdHandler : IRequestHandler<GetLobbyByIdQuery, LobbyDto>
    {
        private readonly ILobbyRepository _lobbyRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetLobbyByIdHandler> _logger;

        public GetLobbyByIdHandler(
            ILobbyRepository lobbyRepository,
            IMapper mapper,
            ILogger<GetLobbyByIdHandler> logger
        )
        {
            _lobbyRepository = lobbyRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<LobbyDto> Handle(GetLobbyByIdQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Start getting lobby with id = '{Id}' by user with login = {Login}",
                query.Id,
                query.Login
            );

            var lobby = await _lobbyRepository.GetByIdAsync(query.Id, cancellationToken)
                ?? throw new LobbyNotFoundException(query.Id);

            var lobbyDto = _mapper.Map<LobbyDto>(lobby);

            _logger.LogInformation(
                "Successfully got lobby with id = '{Id}' by user with login = {Login}",
                query.Id,
                query.Login
            );

            return lobbyDto;
        }
    }
}
