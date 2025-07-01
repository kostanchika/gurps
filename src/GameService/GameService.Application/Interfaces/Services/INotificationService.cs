namespace GameService.Application.Interfaces.Services
{
    public interface INotificationSender
    {
        Task SendNotificationAsync(
            string recipient,
            string title,
            string content,
            Dictionary<string, string> additionalInfo,
            CancellationToken cancellationToken = default
        );
    }
}
