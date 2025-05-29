namespace CommunicationService.Infrastracture.Implementations.ChatService
{
    public interface IConnectionMapper
    {
        void AddConnection(string userId, string connectionId);
        void RemoveConnection(string userId, string connectionId);
        IEnumerable<string> GetUserConnections(string userId);
    }
}
