using CommunicationService.Application.Dto.Message;

namespace CommunicationService.Application.Interfaces.Services
{
    public interface IChatService
    {
        Task NotifyMessageSentAsync(MessageDto message, CancellationToken cancellationToken = default);
        Task NotifyMessageDeletedAsync(string messageId, string chatId, CancellationToken cancellationToken = default);
        Task AddUserToChatAsync(string userLogin, string chatId, CancellationToken cancellationToken = default);
        Task RemoveUserFromChatAsync(string userLogin, string chatId, CancellationToken cancellationToken = default);
    }
}
