using CommunicationService.Application.Interfaces.Repositories;
using CommunicationService.Domain.Entities;
using CommunicationService.Domain.Enums;
using CommunicationService.Infrastracture.Persistense.Mongo.Configurations;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CommunicationService.Infrastracture.Persistense.Mongo
{
    public class MessageRepository : IMessageRepository
    {
        private readonly IMongoCollection<MessageEntity> _messageCollection;

        public MessageRepository(IMongoClient client, IOptions<MongoSettings> mongoSettings)
        {
            var database = client.GetDatabase(mongoSettings.Value.Database);
            _messageCollection = database.GetCollection<MessageEntity>(typeof(MessageEntity).Name);
        }

        public async Task<IEnumerable<MessageEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _messageCollection.Find(_ => true).ToListAsync(cancellationToken);
        }

        public async Task AddAsync(MessageEntity entity, CancellationToken cancellationToken = default)
        {
            await _messageCollection.InsertOneAsync(entity, null, cancellationToken);
        }

        public async Task UpdateAsync(MessageEntity entity, CancellationToken cancellationToken = default)
        {
            await _messageCollection.ReplaceOneAsync(
                message => message.Id == entity.Id,
                entity,
                cancellationToken: cancellationToken
            );
        }

        public async Task RemoveAsync(MessageEntity entity, CancellationToken cancellationToken = default)
        {
            await _messageCollection.DeleteOneAsync(
                message => message.Id == entity.Id,
                cancellationToken
            );
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public async Task<MessageEntity?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            var filter = Builders<MessageEntity>.Filter.Eq(m => m.Id, id);

            return await _messageCollection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            return (int)(await _messageCollection.Find(_ => true).CountDocumentsAsync(cancellationToken: cancellationToken));
        }

        public async Task<IEnumerable<MessageEntity>> GetChatMessagesAsync(
            string chatId,
            int? page,
            int? pageSize,
            CancellationToken cancellationToken = default)
        {
            var filter = Builders<MessageEntity>.Filter.Eq(m => m.ChatId, chatId);

            return await _messageCollection
                .Find(filter)
                .Sort(Builders<MessageEntity>.Sort.Ascending(m => m.CreatedAt))
                .Skip(page.HasValue ? ((page - 1) * pageSize) : 0)
                .Limit(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> CountChatMessagesAsync(string chatId, CancellationToken cancellationToken = default)
        {
            var filter = Builders<MessageEntity>.Filter.Eq(m => m.ChatId, chatId);

            return (int)(await _messageCollection.CountDocumentsAsync(filter, cancellationToken: cancellationToken));
        }

        public async Task MarkMessageAsDeletedAsync(string messageId, CancellationToken cancellationToken = default)
        {
            var filter = Builders<MessageEntity>.Filter.Eq(m => m.Id, messageId);
            var update = Builders<MessageEntity>.Update.Set(m => m.Status, MessageStatus.Deleted);

            await _messageCollection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
        }
    }
}
