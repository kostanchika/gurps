using GameService.Application.DTOs.Game;
using GameService.Application.Features.Game.Commands.CreateAction;
using GameService.Application.Features.Game.Commands.CreateFatigue;
using GameService.Application.Features.Game.Commands.CreateInjury;
using GameService.Application.Features.Game.Commands.GiveCoins;
using GameService.Application.Features.Game.Commands.GiveItem;
using GameService.Application.Features.Game.Commands.GivePoints;
using GameService.Application.Features.Game.Commands.GiveSkill;
using GameService.Application.Features.Game.Commands.GiveTrait;
using GameService.Application.Features.Game.Commands.RemoveFatigue;
using GameService.Application.Features.Game.Commands.RemoveInjury;
using GameService.Application.Features.Game.Commands.StartGame;
using GameService.Application.Features.Game.Commands.TakeAction;
using GameService.Application.Features.Game.Queries.GetGameById;
using GameService.Presentation.Models.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GameService.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GamesController : ControllerBase
    {
        private readonly ISender _sender;

        public GamesController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet("{id}")]
        public Task<GameStateDto> GetGame(
            Guid id,
            CancellationToken cancellationToken
        )
        {
            var login = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            return _sender.Send(new GetGameByIdQuery(id, login), cancellationToken);
        }

        [HttpPost("{id}")]
        public Task StartGame(
            Guid id,
            CancellationToken cancellationToken
        )
        {
            var login = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            return _sender.Send(new StartGameCommand(id, login), cancellationToken);
        }

        [HttpPost("{id}/actions")]
        public async Task CreateAction(
            Guid id,
            CreateActionRequest request,
            CancellationToken cancellationToken
        )
        {
            var login = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var command = new CreateActionCommand(
                id,
                login,
                request.Name,
                request.Content,
                request.Attribute,
                request.Dice
            );

            await _sender.Send(command, cancellationToken);
        }

        [HttpPatch("{id}/actions")]
        public async Task TakeAction(
            Guid id,
            TakeActionRequest request,
            CancellationToken cancellationToken
        )
        {
            var login = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var command = new TakeActionCommand(
                id,
                login,
                request.Items,
                request.Skills,
                request.Traits
            );

            await _sender.Send(command, cancellationToken);
        }

        [HttpPost("{id}/items")]
        public async Task GiveItem(
            Guid id,
            GiveItemRequest request,
            CancellationToken cancellationToken
        )
        {
            var login = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var command = new GiveItemCommand(
                id,
                login,
                request.RecipientLogin,
                request.Name,
                request.Description,
                request.Type,
                request.Quantity,
                request.Price,
                request.Weight,
                request.ArmorBonuses,
                request.DamageBonuses
            );

            await _sender.Send(command, cancellationToken);
        }

        [HttpPost("{id}/skills")]
        public async Task GiveSkill(
            Guid id,
            GiveSkillRequest request,
            CancellationToken cancellationToken
        )
        {
            var login = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var command = new GiveSkillCommand(
                id,
                login,
                request.RecipientLogin,
                request.Name,
                request.AttributeBonus,
                request.Points
            );

            await _sender.Send(command, cancellationToken);
        }

        [HttpPost("{id}/traits")]
        public async Task GiveTrait(
            Guid id,
            GiveTraitRequest request,
            CancellationToken cancellationToken
        )
        {
            var login = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var command = new GiveTraitCommand(
                id,
                login,
                request.RecipientLogin,
                request.Name,
                request.Description,
                request.Points
            );

            await _sender.Send(command, cancellationToken);
        }

        [HttpPost("{id}/injuries")]
        public async Task CreateInjury(
            Guid id,
            GiveInjuryRequest request,
            CancellationToken cancellationToken
        )
        {
            var login = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var command = new CreateInjuryCommand(
                id,
                login,
                request.RecipientLogin,
                request.Name,
                request.HealthPoints
            );

            await _sender.Send(command, cancellationToken);
        }

        [HttpDelete("{id}/injuries")]
        public async Task RemoveInjury(
                    Guid id,
                    RemoveInjuryRequest request,
                    CancellationToken cancellationToken
                )
        {
            var login = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var command = new RemoveInjuryCommand(
                id,
                login,
                request.RecipientLogin,
                request.Name
            );

            await _sender.Send(command, cancellationToken);
        }

        [HttpPost("{id}/fatigues")]
        public async Task CreateFatigue(
            Guid id,
            CreateFatigueRequest request,
            CancellationToken cancellationToken
        )
        {
            var login = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var command = new CreateFatigueCommand(
                id,
                login,
                request.RecipientLogin,
                request.Name,
                request.FatiguePoints
            );

            await _sender.Send(command, cancellationToken);
        }

        [HttpDelete("{id}/fatigues")]
        public async Task RemoveFatigue(
                    Guid id,
                    RemoveFatigueRequest request,
                    CancellationToken cancellationToken
                )
        {
            var login = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var command = new RemoveFatigueCommand(
                id,
                login,
                request.RecipientLogin,
                request.Name
            );

            await _sender.Send(command, cancellationToken);
        }

        [HttpPost("{id}/coins")]
        public async Task GiveCoins(
            Guid id,
            GiveCoinsRequest request,
            CancellationToken cancellationToken
        )
        {
            var login = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var command = new GiveCoinsCommand(
                id,
                login,
                request.RecipientLogin,
                request.Amount
            );

            await _sender.Send(command, cancellationToken);
        }

        [HttpPost("{id}/points")]
        public async Task GivePoints(
            Guid id,
            GivePointsRequest request,
            CancellationToken cancellationToken
        )
        {
            var login = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var command = new GivePointsCommand(
                id,
                login,
                request.RecipientLogin,
                request.Amount
            );

            await _sender.Send(command, cancellationToken);
        }
    }
}
