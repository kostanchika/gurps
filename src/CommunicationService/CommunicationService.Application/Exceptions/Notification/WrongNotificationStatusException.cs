using CommunicationService.Domain.Enums;
namespace CommunicationService.Application.Exceptions.Notification
{
    public class WrongNotificationStatusException(string userLogin, string notificationId, NotificationStatus notificationStatus)
        : ConflictOperationException(
                $"User with login = {userLogin} tried to set invalid notification status ({notificationStatus})" +
                $" for notification with id = {notificationId}"
            )
    {
        public string UserLogin { get; set; } = userLogin;
        public string NotificationId { get; set; } = notificationId;
        public NotificationStatus NotificationStatus { get; set; } = notificationStatus;
    }
}
