using GameService.Application.Exceptions.Lobby;
using GameService.Application.Interfaces.Repositories;
using GameService.Application.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GameService.Application.Features.Lobby.Commands.DeleteLobby
{
    public class DeleteLobbyHandler : IRequestHandler<DeleteLobbyCommand, Unit>
    {
        private readonly ILobbyRepository _lobbyRepository;
        private readonly ILobbyService _lobbyService;
        private readonly ILogger<DeleteLobbyHandler> _logger;

        public DeleteLobbyHandler(
            ILobbyRepository lobbyRepository,
            ILobbyService lobbyService,
            ILogger<DeleteLobbyHandler> logger
        )
        {
            _lobbyRepository = lobbyRepository;
            _lobbyService = lobbyService;
            _logger = logger;
        }

        public async Task<Unit> Handle(DeleteLobbyCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Start deleting lobby with id = '{Id}' by user with login = '{Login}'",
                command.LobbyId,
                command.Login
            );

            var lobby = await _lobbyRepository.GetByIdAsync(command.LobbyId, cancellationToken)
                ?? throw new LobbyNotFoundException(command.LobbyId);

            if (lobby.MasterLogin != command.Login)
            {
                throw new UserIsNotOwnerException(command.Login, command.LobbyId);
            }

            await _lobbyRepository.RemoveAsync(lobby, cancellationToken);
            await _lobbyService.NotifyLobbiesUpdatedAsync(cancellationToken);

            _logger.LogInformation(
                "Successfully deleted lobby with id = '{Id}' by user with login = '{Login}'",
                command.LobbyId,
                command.Login
            );

            return Unit.Value;
        }
    }
}
