using System.Collections.Concurrent;

namespace CommunicationService.Infrastracture.Implementations.ChatService
{
    public class ConnectionMapper : IConnectionMapper
    {
        private readonly ConcurrentDictionary<string, HashSet<string>> _connections = new();

        public void AddConnection(string userId, string connectionId)
        {
            _connections.AddOrUpdate(userId, [connectionId],
                (_, existingConnections) =>
                {
                    existingConnections.Add(connectionId);
                    return existingConnections;
                });
        }

        public void RemoveConnection(string userId, string connectionId)
        {
            if (_connections.TryGetValue(userId, out var connections))
            {
                connections.Remove(connectionId);
                if (connections.Count == 0) _connections.TryRemove(userId, out _);
            }
        }

        public IEnumerable<string> GetUserConnections(string userId)
        {
            return _connections.TryGetValue(userId, out var connections) ? connections : Enumerable.Empty<string>();
        }
    }
}
