namespace GameService.Application.Exceptions.Character
{
    internal class CharacterOwnershipException(string login, Guid characterId)
        : BadRequestException($"Character with id = '{characterId}' doesn't belong to user with login = '{login}'")
    {
    }
}
