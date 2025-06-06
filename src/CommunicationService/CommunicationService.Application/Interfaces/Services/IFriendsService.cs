namespace CommunicationService.Application.Interfaces.Services
{
    public interface IFriendsService
    {
        Task<bool> AreFriendsAsync(string firstLogin, string secondLogin, CancellationToken cancellationToken = default);
    }
}
