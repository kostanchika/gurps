namespace CommunicationService.Infrastracture.Implementations.NotificationService
{
    public class NotificationSettings
    {
        public string Host { get; set; } = null!;
        public string User { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Queue { get; set; } = null!;
    }
}
