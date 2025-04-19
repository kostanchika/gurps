namespace UsersService.Application.DTOs.Character
{
    public record CharacterFiltersDto(
        string? UserLogin,
        int? UserId,
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
    );
}
