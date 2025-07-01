namespace GameService.Presentation.Models.Requests
{
    public record ConnectToLobbyRequest(
        Guid CharacterId,
        string? Password
    );
}
