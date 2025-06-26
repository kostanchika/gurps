namespace GameService.Presentation.Models.Requests
{
    public record RemoveInjuryRequest(
        string RecipientLogin,
        string Name
    );
}
