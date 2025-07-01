namespace GameService.Presentation.Models.Requests
{
    public record GiveCoinsRequest(
        string RecipientLogin,
        int Amount
    );
}
