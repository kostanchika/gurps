namespace UsersService.Application.Exceptions.Character
{
    public class CharacterNotFoundException(int characterId)
        : NotFoundException($"Character with Id = '{characterId}' was not found")
    {
        public int CharacterId { get; set; } = characterId;
    }
}
