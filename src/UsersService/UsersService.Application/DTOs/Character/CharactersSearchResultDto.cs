namespace UsersService.Application.DTOs.Character
{
    public class CharactersSearchResultDto
    {
        public int TotalPages { get; set; }
        public IEnumerable<CharacterDto> Characters { get; set; } = [];
    }
}
