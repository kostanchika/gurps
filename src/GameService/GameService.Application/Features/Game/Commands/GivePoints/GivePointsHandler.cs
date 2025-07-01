using GameService.Application.Exceptions.Character;
using GameService.Application.Exceptions.Game;
using GameService.Application.Exceptions.Lobby;
using GameService.Application.Interfaces.Repositories;
using GameService.Application.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GameService.Application.Features.Game.Commands.GivePoints
{
    public class GivePointsHandler : IRequestHandler<GivePointsCommand, Unit>
    {
        private readonly ILobbyRepository _lobbyRepository;
        private readonly ICharacterRepository _characterRepository;
        private readonly ILobbyService _lobbyService;
        private readonly INotificationSender _notificationSender;
        private readonly ILogger<GivePointsHandler> _logger;

        public GivePointsHandler(
            ILobbyRepository lobbyRepository,
            ICharacterRepository characterRepository,
            ILobbyService lobbyService,
            INotificationSender notificationSender,
            ILogger<GivePointsHandler> logger
        )
        {
            _lobbyRepository = lobbyRepository;
            _characterRepository = characterRepository;
            _lobbyService = lobbyService;
            _notificationSender = notificationSender;
            _logger = logger;
        }

        public async Task<Unit> Handle(GivePointsCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Start giving {Points} points to player with login = '{Login}' in lobby with id = '{Id}'",
                command.Amount,
                command.RecipientLogin,
                command.LobbyId
            );

            var lobby = await _lobbyRepository.GetByIdAsync(command.LobbyId, cancellationToken)
                ?? throw new LobbyNotFoundException(command.LobbyId);

            if (lobby.MasterLogin != command.Login)
            {
                throw new UserIsNotOwnerException(command.Login, command.LobbyId);
            }

            var player = lobby.Players.FirstOrDefault(p => p.Login == command.RecipientLogin)
                ?? throw new UserIsNotParticipantException(command.RecipientLogin, command.LobbyId);

            var character = await _characterRepository.GetByIdAsync(player.CharacterId, cancellationToken)
                ?? throw new CharacterNotFoundException(player.CharacterId);

            if (lobby.IsEnded)
            {
                throw new InvalidGameStateException(command.LobbyId);
            }

            character.SummaryPoints += command.Amount;

            await _lobbyRepository.UpdateAsync(lobby, cancellationToken);
            await _lobbyRepository.SaveChangesAsync(cancellationToken);

            await _lobbyService.NotifyLobbyUpdatedAsync(lobby.Id, cancellationToken);

            await _notificationSender.SendNotificationAsync(
                player.Login,
                "You got points",
                $"You got {command.Amount} points",
                new Dictionary<string, string> { { "LobbyId", lobby.Id.ToString() } },
                cancellationToken
            );

            _logger.LogInformation(
                "Successfully gave {Points} points to player with login = '{Login}' in lobby with id = '{Id}'",
                command.Amount,
                command.RecipientLogin,
                command.LobbyId
            );

            return Unit.Value;
        }
    }
}
