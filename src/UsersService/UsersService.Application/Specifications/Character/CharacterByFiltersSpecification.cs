using GURPS.Character.Entities;
using System.Linq.Expressions;
using UsersService.Application.DTOs.Character;
using UsersService.Domain.Interfaces;

namespace UsersService.Application.Specifications.Character
{
    public class CharacterByFiltersSpecification(CharacterFiltersDto filters)
        : ISpecification<CharacterEntity>
    {
        public Expression<Func<CharacterEntity, bool>>? Criteria
            => character => character.Name.Contains(filters.Name!) &&
                            character.World.Contains(filters.World!) &&
                            character.Appearence.Age.Contains(filters.Age!) &&
                            character.Appearence.BirthDate.Contains(filters.BirthDate!) &&
                            character.Appearence.Eyes.Contains(filters.Eyes!) &&
                            character.Appearence.Hair.Contains(filters.Hair!) &&
                            character.Appearence.Skin.Contains(filters.Skin!) &&
                            character.Appearence.Height.Contains(filters.Height!) &&
                            character.Appearence.Weight.Contains(filters.Weight!) &&
                            character.Appearence.Race.Contains(filters.Race!) &&
                            character.Appearence.Religion.Contains(filters.Religion!) &&
                            (filters.UserId == null || character.UserId == filters.UserId);

        public List<Expression<Func<CharacterEntity, object>>>? Includes
            => null;

        public int? Page { get; set; } = filters.Page;
        public int? PageSize { get; set; } = filters.PageSize;
    }
}
