namespace GameService.Presentation.Models.Requests
{
    public record CreateFatigueRequest(
        string RecipientLogin,
        string Name,
        int FatiguePoints
    );
}
