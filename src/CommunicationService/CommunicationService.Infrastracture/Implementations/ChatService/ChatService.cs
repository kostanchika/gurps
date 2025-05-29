using CommunicationService.Application.Dto.Message;
using CommunicationService.Application.Interfaces.Services;
using Microsoft.AspNetCore.SignalR;

namespace CommunicationService.Infrastracture.Implementations.ChatService
{
    public class ChatService : IChatService
    {
        private readonly IHubContext<ChatHub> _hub;
        private readonly IConnectionMapper _connectionMapper;

        public ChatService(IHubContext<ChatHub> hub, IConnectionMapper connectionMapper)
        {
            _hub = hub;
            _connectionMapper = connectionMapper;
        }

        public async Task AddUserToChatAsync(string userLogin, string chatId, CancellationToken cancellationToken = default)
        {
            foreach (var connection in _connectionMapper.GetUserConnections(userLogin))
            {
                await _hub.Groups.AddToGroupAsync(connection, chatId, cancellationToken);
            }
            await _hub.Clients.User(userLogin).SendAsync("AddedToChat", cancellationToken);
        }

        public async Task RemoveUserFromChatAsync(string userLogin, string chatId, CancellationToken cancellationToken = default)
        {
            foreach (var connection in _connectionMapper.GetUserConnections(userLogin))
            {
                await _hub.Groups.RemoveFromGroupAsync(connection, chatId, cancellationToken);
            }
            await _hub.Clients.User(userLogin).SendAsync("RemovedFromChat", cancellationToken);
        }

        public async Task NotifyMessageDeletedAsync(string messageId, string chatId, CancellationToken cancellationToken = default)
        {
            await _hub.Clients.Group(chatId).SendAsync("DeleteMessage", messageId, cancellationToken);
        }

        public async Task NotifyMessageSentAsync(MessageDto message, CancellationToken cancellationToken = default)
        {
            await _hub.Clients.Group(message.ChatId).SendAsync("ReceiveMessage", message, cancellationToken);
        }
    }
}
