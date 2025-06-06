using CommunicationService.Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace CommunicationService.Infrastracture.Implementations.ChatService
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IConnectionMapper _connectionMapper;
        private readonly IChatRepository _chatRepository;

        public ChatHub(IConnectionMapper connectionMapper, IChatRepository chatRepository)
        {
            _connectionMapper = connectionMapper;
            _chatRepository = chatRepository;
        }

        public override async Task OnConnectedAsync()
        {
            var userLogin = Context.UserIdentifier;
            _connectionMapper.AddConnection(userLogin, Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userLogin = Context.UserIdentifier;
            _connectionMapper.RemoveConnection(userLogin, Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task JoinChat(string chatId)
        {
            var chat = await _chatRepository.GetByIdAsync(chatId)
                ?? throw new HubException("Chat is not found");

            var userId = Context.UserIdentifier;

            if (!chat.Participants.Contains(userId))
            {
                throw new HubException("Forbidden");
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, chatId);
        }

        public async Task LeaveChat(string chatId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId);
        }
    }
}
