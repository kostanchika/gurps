using GameService.Application.Models;
using GURPS.Character.Entities;
using MediatR;

namespace GameService.Application.Features.Character.Queries.GetCharacters
{
    public record GetCharactersQuery(
        string? UserLogin,
        string? Name = "",
        string? World = "",
        string? Age = "",
        string? BirthDate = "",
        string? Eyes = "",
        string? Hair = "",
        string? Skin = "",
        string? Height = "",
        string? Weight = "",
        string? Race = "",
        string? Religion = "",
        int Page = 1,
        int PageSize = 10
    ) : IRequest<PagedResultDto<CharacterEntity>>;
}
