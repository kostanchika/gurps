using GameService.Application.DTOs.Game;
using GameService.Application.Features.Character.Commands.CreateCharacter;
using GameService.Application.Features.Character.Commands.DeleteCharcater;
using GameService.Application.Features.Character.Commands.UpdateCharacter;
using GameService.Application.Features.Character.Queries.GetCharacterById;
using GameService.Application.Features.Character.Queries.GetCharacters;
using GameService.Application.Features.Character.Queries.GetPointsConfiguration;
using GameService.Presentation.Models.Requests;
using GURPS.Character.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GameService.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CharactersController : ControllerBase
    {
        private readonly ISender _sender;

        public CharactersController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<IEnumerable<CharacterEntity>> GetCharacters(
            [FromQuery] GetCharactersQuery query,
            CancellationToken cancellationToken    
        )
        {
            var pagedResult = await _sender.Send(query, cancellationToken);

            Response.Headers["X-Total-Count"] = pagedResult.Count.ToString();

            return pagedResult.Items;
        }

        [HttpGet("{id}")]
        public Task<CharacterEntity> GetCharacterById(
            Guid id,
            CancellationToken cancellationToken
        )
        {
            return _sender.Send(new GetCharacterByIdQuery(id), cancellationToken);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<CharacterEntity>> CreateCharacter(
            CreateCharacterRequest request,
            CancellationToken cancellationToken
        )
        {
            var login = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var command = new CreateCharacterCommand(
                login,
                request.Name,
                request.Base64Avatar,
                request.World,
                request.History,
                request.Appearence,
                request.Attributes
            );

            var id = await _sender.Send(command, cancellationToken);
            var character = await _sender.Send(new GetCharacterByIdQuery(id), cancellationToken);

            return CreatedAtAction(nameof(GetCharacterById), new { Id = id }, character);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<CharacterEntity> UpdateCharacter(
            Guid id,
            CreateCharacterRequest request,
            CancellationToken cancellationToken
        )
        {
            var login = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var command = new UpdateCharacterCommand(
                login,
                id,
                request.Name,
                request.Base64Avatar,
                request.World,
                request.History,
                request.Appearence,
                request.Attributes
            );

            await _sender.Send(command, cancellationToken);
            var character = await _sender.Send(new GetCharacterByIdQuery(id), cancellationToken);

            return character;
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<CharacterEntity> DeleteCharacter(
            Guid id,
            CancellationToken cancellationToken
        )
        {
            var login = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var command = new DeleteCharacterCommand(
                login,
                id
            );

            var character = await _sender.Send(new GetCharacterByIdQuery(id), cancellationToken);

            await _sender.Send(command, cancellationToken);

            return character;
        }

        [HttpGet("configuration")]
        public Task<PointsConfigurationDto> GetPointsConfiguration(
            CancellationToken cancellationToken    
        )
        {
            return _sender.Send(new GetPointsConfigurationQuery(), cancellationToken);
        }
    }
}
