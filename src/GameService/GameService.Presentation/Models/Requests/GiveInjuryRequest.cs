namespace GameService.Presentation.Models.Requests
{
    public record GiveInjuryRequest(
        string RecipientLogin,
        string Name,
        int HealthPoints
    );
}
