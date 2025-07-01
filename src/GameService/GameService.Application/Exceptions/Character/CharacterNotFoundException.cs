namespace GameService.Application.Exceptions.Character
{
    internal class CharacterNotFoundException(Guid characterId)
        : EntityNotFoundException($"Character with id = '{characterId}' was not found")
    {
    }
}
