namespace CommunicationService.Application.Exceptions.Notification
{
    public class UnauthorizedNotificationException(string userLogin, string notificationId)
        : ForbiddenOperationException(
                $"User with login = '{userLogin}' tried to access notification with id = '{notificationId}' what does not belong to him"
            )
    {
        public string UserLogin { get; set; } = userLogin;
        public string NotificationId { get; set; } = notificationId;
    }
}
