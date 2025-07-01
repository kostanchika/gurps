using GameService.Application.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using GameService.Domain.Entities;
using GameService.Application.Interfaces.Services;
using GameService.Application.Exceptions.Lobby;

namespace GameService.Application.Features.Lobby.Commands.CreateLobby
{
    public class CreateLobbyHandler : IRequestHandler<CreateLobbyCommand, Guid>
    {
        private readonly ILobbyRepository _lobbyRepository;
        private readonly ILobbyService _lobbyService;
        private readonly ILogger<CreateLobbyHandler> _logger;

        public CreateLobbyHandler(
            ILobbyRepository lobbyRepository,
            ILobbyService lobbyService,
            ILogger<CreateLobbyHandler> logger
        )
        {
            _lobbyRepository = lobbyRepository;
            _lobbyService = lobbyService;
            _logger = logger;
        }

        public async Task<Guid> Handle(CreateLobbyCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Start creating lobby by user with login = '{Login}'", command.Login);

            var existingLobby = await _lobbyRepository.GetByUserLoginAsync(command.Login, cancellationToken);

            if (existingLobby != null)
            {
                throw new DuplicateLobbyException(command.Login);
            }

            var lobby = new LobbyEntity
            {
                MasterLogin = command.Login,
                Players = [],
                Actions = [],
                Password = command.Password
            };

            await _lobbyRepository.AddAsync(lobby, cancellationToken);
            await _lobbyRepository.SaveChangesAsync(cancellationToken);

            await _lobbyService.NotifyLobbiesUpdatedAsync(cancellationToken);

            _logger.LogInformation("Successfully created lobby by user with login = '{Login}'", command.Login);

            return lobby.Id;
        }
    }
}
