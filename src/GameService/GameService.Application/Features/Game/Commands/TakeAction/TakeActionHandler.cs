using GameService.Application.Exceptions.Game;
using GameService.Application.Exceptions.Lobby;
using GameService.Application.Interfaces.Repositories;
using GameService.Application.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GameService.Application.Features.Game.Commands.TakeAction
{
    public class TakeActionHandler : IRequestHandler<TakeActionCommand, Unit>
    {
        private readonly ILobbyRepository _lobbyRepository;
        private readonly ILobbyService _lobbyService;
        private readonly IConfirmationService _confirmationService;
        private readonly ILogger<TakeActionHandler> _logger;
        private static readonly Random _random = new ();

        public TakeActionHandler(
            ILobbyRepository lobbyRepository,
            ILobbyService lobbyService,
            IConfirmationService confirmationService,
            ILogger<TakeActionHandler> logger
        )
        {
            _lobbyRepository = lobbyRepository;
            _lobbyService = lobbyService;
            _confirmationService = confirmationService;
            _logger = logger;
        }

        public async Task<Unit> Handle(TakeActionCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Start taking action in lobby with id = '{Id}' by player with login = '{Login}'",
                command.LobbyId,
                command.Login
            );

            var lobby = await _lobbyRepository.GetByIdAsync(command.LobbyId, cancellationToken)
               ?? throw new LobbyNotFoundException(command.LobbyId);

            if (lobby.IsEnded)
            {
                throw new InvalidGameStateException(command.LobbyId);
            }

            var lastAction = lobby.Actions.FirstOrDefault(a => a.Dice.HasValue && !a.PlayerDice.HasValue)
                ?? throw new InvalidGameStateException(lobby.Id);

            var player = lobby.Players.FirstOrDefault(p => p.Login == command.Login)
                ?? throw new UserIsNotParticipantException(command.Login, command.LobbyId);

            lastAction.Player = player;

            await _lobbyRepository.UpdateAsync(lobby, cancellationToken);
            await _lobbyRepository.SaveChangesAsync(cancellationToken);

            await _lobbyService.NotifyLobbyUpdatedAsync(lobby.Id, cancellationToken);

            await _lobbyService.ShowDiceAnimationAsync(lobby.Id, cancellationToken);

            var dice = _random.Next(1, 19);

            await Task.Delay(3000, cancellationToken);

            await _lobbyService.ShowDiceValueAsync(lobby.Id, dice, cancellationToken);

            lastAction.Player = player;
            lastAction.PlayerDice = dice;

            await _lobbyRepository.UpdateAsync(lobby, cancellationToken);
            await _lobbyRepository.SaveChangesAsync(cancellationToken);

            await _lobbyService.NotifyLobbyUpdatedAsync(lobby.Id, cancellationToken);

            _logger.LogInformation(
                "Successfully took action in lobby with id = '{Id}' by player with login = '{Login}'",
                command.LobbyId,
                command.Login
            );

            return Unit.Value;
        }
    }
}
