using GameService.Application.Exceptions.Character;
using GameService.Application.Exceptions.Game;
using GameService.Application.Exceptions.Lobby;
using GameService.Application.Interfaces.Repositories;
using GameService.Application.Interfaces.Services;
using GameService.Domain.Entities;
using GURPS.Character.Entities.CharacterProperties;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GameService.Application.Features.Game.Commands.GiveSkill
{
    public class GiveSkillHandler : IRequestHandler<GiveSkillCommand, Unit>
    {
        private readonly ILobbyRepository _lobbyRepository;
        private readonly ICharacterRepository _characterRepository;
        private readonly IConfirmationService _confirmationService;
        private readonly INotificationSender _notificationSender;
        private readonly ILobbyService _lobbyService;
        private readonly ILogger<GiveSkillHandler> _logger;

        public GiveSkillHandler(
            ILobbyRepository lobbyRepository,
            ICharacterRepository characterRepository,
            IConfirmationService confirmationService,
            INotificationSender notificationSender,
            ILobbyService lobbyService,
            ILogger<GiveSkillHandler> logger
        )
        {
            _lobbyRepository = lobbyRepository;
            _characterRepository = characterRepository;
            _confirmationService = confirmationService;
            _notificationSender = notificationSender;
            _lobbyService = lobbyService;
            _logger = logger;
        }

        public async Task<Unit> Handle(GiveSkillCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Start creating skill in lobby with id = '{Id}'", command.LobbyId);

            var lobby = await _lobbyRepository.GetByIdAsync(command.LobbyId, cancellationToken)
                ?? throw new LobbyNotFoundException(command.LobbyId);

            if (lobby.MasterLogin != command.Login)
            {
                throw new UserIsNotOwnerException(command.Login, command.LobbyId);
            }

            if (lobby.IsEnded)
            {
                throw new InvalidGameStateException(command.LobbyId);
            }

            var player = lobby.Players.FirstOrDefault(p => p.Login == command.RecipientLogin)
                ?? throw new UserIsNotParticipantException(command.RecipientLogin, command.LobbyId);

            var character = await _characterRepository.GetByIdAsync(player.CharacterId, cancellationToken)
                ?? throw new CharacterNotFoundException(player.CharacterId);

            if (character.SummaryPoints < command.Points)
            {
                throw new NotEnoughPointsException(character.Id);
            }

            var skill = new Skill
            {
                Name = command.Name,
                AttributeBonus = command.AttributeBonus,
                Points = command.Points,
            };

            var result = await _confirmationService.SendGetSkillConfirmationAsync(
                command.RecipientLogin,
                skill,
                cancellationToken
            );

            if (result)
            {
                character.SummaryPoints -= command.Points;

                character.Skills.Add(skill);

                var giveSkillAction = new ActionEntity
                {
                    Name = "New skill",
                    Content = $"Player {player.Login} got {skill.Name}",
                    Player = player
                };

                lobby.Actions.Add(giveSkillAction);

                await _lobbyRepository.UpdateAsync(lobby, cancellationToken);
                await _lobbyRepository.SaveChangesAsync(cancellationToken);

                var attributeBonus = skill.AttributeBonus;

                var description = new Dictionary<string, string>
                {
                    {
                        attributeBonus.Attribute.ToString(),
                        $"+{attributeBonus.Bonus}"
                    },
                    { "LobbyId", lobby.Id.ToString() }
                };

                await _notificationSender.SendNotificationAsync(
                    player.Login,
                    "New skill",
                    $"You got {skill.Name}",
                    description,
                    cancellationToken
                );
            }

            await _lobbyService.NotifyLobbyUpdatedAsync(lobby.Id, cancellationToken);

            _logger.LogInformation("Successfully created skill in lobby with id = '{Id}'", command.LobbyId);

            return Unit.Value;
        }
    }
}
