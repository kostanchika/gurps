namespace GameService.Presentation.Models.Requests
{
    public record GiveTraitRequest(
        string RecipientLogin,
        string Name,
        string Description,
        int Points
    );
}
