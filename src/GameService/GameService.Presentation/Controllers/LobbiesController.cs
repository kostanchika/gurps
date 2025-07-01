using GameService.Application.Features.Lobby.Commands.ConnectToLobby;
using GameService.Application.Features.Lobby.Commands.CreateLobby;
using GameService.Application.Features.Lobby.Commands.DeleteLobby;
using GameService.Application.Features.Lobby.Commands.DisconnectFromLobby;
using GameService.Application.Features.Lobby.Queries.GetLobbies;
using GameService.Application.Features.Lobby.Queries.GetLobbyById;
using GameService.Application.Models.Lobby;
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
    public class LobbiesController : ControllerBase
    {
        private readonly ISender _sender;

        public LobbiesController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<IEnumerable<LobbyDto>> GetLobbies(
            [FromQuery] GetLobbiesQuery query,
            CancellationToken cancellationToken = default
        )
        {
            var pagedResult = await _sender.Send(query, cancellationToken);

            Response.Headers["X-Total-Count"] = pagedResult.Count.ToString();

            return pagedResult.Items;
        }

        [HttpGet("{id}")]
        public async Task<LobbyDto> GetLobby(
            Guid id,
            CancellationToken cancellationToken = default
        )
        {
            var login = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            return await _sender.Send(new GetLobbyByIdQuery(login, id), cancellationToken);
        }

        [HttpPost]
        public async Task<ActionResult<LobbyDto>> CreateLobby(
            CreateLobbyRequest request,
            CancellationToken cancellationToken
        )
        {
            var login = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var command = new CreateLobbyCommand(
                login,
                request.Password
            );

            var id = await _sender.Send(command, cancellationToken);

            var lobby = await _sender.Send(new GetLobbyByIdQuery(login, id), cancellationToken);

            return CreatedAtAction(nameof(GetLobby), new { Id = id }, lobby);
        }

        [HttpDelete("{id}")]
        public async Task DeleteLobby(
            Guid id,
            CancellationToken cancellationToken
        )
        {
            var login = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var command = new DeleteLobbyCommand(login, id);

            await _sender.Send(command, cancellationToken);
        }

        [HttpPost("{id}/participants")]
        public async Task<LobbyDto> ConnectToLobby(
            Guid id,
            ConnectToLobbyRequest request,
            CancellationToken cancellationToken
        )
        {
            var login = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var command = new ConnectToLobbyCommand(
                login,
                request.CharacterId,
                id,
                request.Password
            );

            await _sender.Send(command, cancellationToken);
            var lobby = await _sender.Send(new GetLobbyByIdQuery(login, id), cancellationToken);

            return lobby;
        }

        [HttpDelete("{id}/participants")]
        public async Task<LobbyDto> DisconnectFromLobby(
            Guid id,
            CancellationToken cancellationToken
        )
        {
            var login = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            await _sender.Send(new DisconnectFromLobbyCommand(login, id), cancellationToken);
            var lobby = await _sender.Send(new GetLobbyByIdQuery(login, id), cancellationToken);

            return lobby;
        }
    }
}
