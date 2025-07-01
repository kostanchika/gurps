namespace GameService.Presentation.Models.Requests
{
    public record GivePointsRequest(
        string RecipientLogin,
        int Amount
    );
}
