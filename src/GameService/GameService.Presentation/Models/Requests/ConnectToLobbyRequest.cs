namespace GameService.Presentation.Models.Requests
{
    public record ConnectToLobbyRequest(
        int CharacterId,
        string? Password
    );
}
