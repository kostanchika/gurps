using CommunicationService.Application.Interfaces.Repositories;
using CommunicationService.Domain.Entities;
using CommunicationService.Infrastracture.Persistense.Mongo.Configurations;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CommunicationService.Infrastracture.Persistense.Mongo
{
    public class ChatRepository : IChatRepository
    {
        private readonly IMongoCollection<ChatEntity> _chatCollection;

        public ChatRepository(IMongoClient client, IOptions<MongoSettings> mongoSettings)
        {
            var database = client.GetDatabase(mongoSettings.Value.Database);
            _chatCollection = database.GetCollection<ChatEntity>(typeof(ChatEntity).Name);
        }

        public async Task<IEnumerable<ChatEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _chatCollection.Find(_ => true).ToListAsync(cancellationToken);
        }

        public async Task AddAsync(ChatEntity entity, CancellationToken cancellationToken = default)
        {
            await _chatCollection.InsertOneAsync(entity, null, cancellationToken);
        }

        public async Task UpdateAsync(ChatEntity entity, CancellationToken cancellationToken = default)
        {
            await _chatCollection.ReplaceOneAsync(
                chat => chat.Id == entity.Id,
                entity,
                cancellationToken: cancellationToken
            );
        }

        public async Task RemoveAsync(ChatEntity entity, CancellationToken cancellationToken = default)
        {
            await _chatCollection.DeleteOneAsync(
                chat => chat.Id == entity.Id,
                cancellationToken
            );
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public async Task<IEnumerable<ChatEntity>> GetUserChats(
            string userLogin,
            string? chatName,
            int? pageNumber,
            int? pageSize,
            string? sortBy,
            string? sortType,
            CancellationToken cancellationToken = default
        )
        {
            var filter = Builders<ChatEntity>.Filter.AnyEq(c => c.Participants, userLogin);

            if (!string.IsNullOrEmpty(chatName))
            {
                filter &= Builders<ChatEntity>.Filter.Regex(c => c.Name, new BsonRegularExpression(chatName, "i"));
            }

            var query = _chatCollection.Find(filter);

            if (!string.IsNullOrEmpty(sortBy))
            {
                var sortDefinition = sortType?.ToLower() == "desc"
                    ? Builders<ChatEntity>.Sort.Descending(sortBy)
                    : Builders<ChatEntity>.Sort.Ascending(sortBy);

                query = query.Sort(sortDefinition);
            }

            return await query
                .Skip(pageNumber.HasValue ? ((pageNumber - 1) * pageSize) : 0)
                .Limit(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> CountUserChats(string userLogin, string? chatName, CancellationToken cancellationToken = default)
        {
            var filter = Builders<ChatEntity>.Filter.AnyEq(c => c.Participants, userLogin);

            if (!string.IsNullOrEmpty(chatName))
            {
                filter &= Builders<ChatEntity>.Filter.Regex(c => c.Name, new BsonRegularExpression(chatName, "i"));
            }

            return (int)(await _chatCollection.CountDocumentsAsync(filter, cancellationToken: cancellationToken));
        }

        public async Task<ChatEntity?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            var filter = Builders<ChatEntity>.Filter.Eq(c => c.Id, id);

            return await _chatCollection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            return (int)(await _chatCollection.Find(_ => true).CountDocumentsAsync(cancellationToken: cancellationToken));
        }
    }
}
