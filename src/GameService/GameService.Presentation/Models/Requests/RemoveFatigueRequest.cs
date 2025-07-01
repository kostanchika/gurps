namespace GameService.Presentation.Models.Requests
{
    public record RemoveFatigueRequest(
        string RecipientLogin,
        string Name
    );
}
