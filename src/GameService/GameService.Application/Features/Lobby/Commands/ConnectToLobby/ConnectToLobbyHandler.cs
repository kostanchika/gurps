using GameService.Application.Exceptions.Character;
using GameService.Application.Exceptions.Lobby;
using GameService.Application.Interfaces.Repositories;
using GameService.Application.Interfaces.Services;
using GameService.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GameService.Application.Features.Lobby.Commands.ConnectToLobby
{
    public class ConnectToLobbyHandler : IRequestHandler<ConnectToLobbyCommand, Guid>
    {
        private readonly ILobbyRepository _lobbyRepository;
        private readonly ICharacterService _characterService;
        private readonly ILobbyService _lobbyService;
        private readonly ILogger<ConnectToLobbyHandler> _logger;

        public ConnectToLobbyHandler(
            ILobbyRepository lobbyRepository,
            ICharacterService characterService,
            ILobbyService lobbyService,
            ILogger<ConnectToLobbyHandler> logger
        )
        {
            _lobbyRepository = lobbyRepository;
            _characterService = characterService;
            _lobbyService = lobbyService;
            _logger = logger;
        }

        public async Task<Guid> Handle(ConnectToLobbyCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Start connecting user with login = '{Login}' to lobby with id = '{Id}'",
                command.Login,
                command.LobbyId
            );

            var lobby = await _lobbyRepository.GetByIdAsync(command.LobbyId, cancellationToken)
                ?? throw new LobbyNotFoundException(command.LobbyId);

            var userLobby = await _lobbyRepository.GetByUserLoginAsync(command.Login, cancellationToken);

            if (userLobby != null)
            {
                throw new DuplicateLobbyException(command.Login);
            }

            if (!string.IsNullOrEmpty(lobby.Password) && lobby.Password != command.Password)
            {
                throw new InvalidPasswordException(command.Login, lobby.Id);
            }

            if (lobby.Players.Any(p => p.Login == command.Login))
            {
                throw new DuplicateParticipantException(command.Login);
            }

            if (lobby.Actions.Count > 0)
            {
                throw new LobbyIsInGameException();
            }

            var character = await _characterService.GetCharacterAsync(
                command.CharacterId,
                cancellationToken
            );

            //if (character.UserLogin != command.Login)
            //{
            //throw new CharacterOwnershipException(command.Login, command.CharacterId);
            //}

            var player = new PlayerEntity
            {
                Login = command.Login,
                Character = character,
            };

            lobby.Players.Add(player);

            await _lobbyRepository.UpdateAsync(lobby, cancellationToken);
            await _lobbyRepository.SaveChangesAsync(cancellationToken);

            await _lobbyService.ConnectPlayerAsync(lobby.Id, command.Login, cancellationToken);
            await _lobbyService.NotifyLobbiesUpdatedAsync(cancellationToken);

            _logger.LogInformation(
                "Successfully conected user with login = '{Login}' to lobby with id = '{Id}'",
                command.Login,
                command.LobbyId
            );

            return lobby.Id;
        }
    }
}
