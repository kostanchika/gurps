namespace CommunicationService.Application.Exceptions.Notification
{
    public class NotificationNotFoundException(string notificationId)
        : EntityNotFoundException($"Notification with id = {notificationId} was not found")
    {
        public string NotificationId { get; set; } = notificationId;
    }
}
