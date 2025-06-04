using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace CommunicationService.Infrastracture.Implementations.NotificationService
{
    [Authorize]
    public class NotificationHub : Hub
    {
    }
}
