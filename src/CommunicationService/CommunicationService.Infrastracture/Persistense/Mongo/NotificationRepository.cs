using CommunicationService.Application.Interfaces.Repositories;
using CommunicationService.Domain.Entities;
using CommunicationService.Domain.Enums;
using CommunicationService.Infrastracture.Persistense.Mongo.Configurations;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CommunicationService.Infrastracture.Persistense.Mongo
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly IMongoCollection<NotificationEntity> _notificationCollection;

        public NotificationRepository(IMongoClient client, IOptions<MongoSettings> mongoSettings)
        {
            var database = client.GetDatabase(mongoSettings.Value.Database);
            _notificationCollection = database.GetCollection<NotificationEntity>(typeof(NotificationEntity).Name);
        }

        public async Task<IEnumerable<NotificationEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _notificationCollection.Find(_ => true).ToListAsync(cancellationToken);
        }

        public async Task AddAsync(NotificationEntity entity, CancellationToken cancellationToken = default)
        {
            await _notificationCollection.InsertOneAsync(entity, null, cancellationToken);
        }

        public async Task UpdateAsync(NotificationEntity entity, CancellationToken cancellationToken = default)
        {
            await _notificationCollection.ReplaceOneAsync(
                chat => chat.Id == entity.Id,
                entity,
                cancellationToken: cancellationToken
            );
        }

        public async Task RemoveAsync(NotificationEntity entity, CancellationToken cancellationToken = default)
        {
            await _notificationCollection.DeleteOneAsync(
                chat => chat.Id == entity.Id,
                cancellationToken
            );
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public async Task<NotificationEntity?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            var filter = Builders<NotificationEntity>.Filter.Eq(n => n.Id, id);

            return await _notificationCollection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            return (int)(await _notificationCollection.Find(_ => true).CountDocumentsAsync(cancellationToken: cancellationToken));
        }

        public async Task<IEnumerable<NotificationEntity>> GetNotificationsByUserLoginAsync(
            string userLogin,
            bool onlyNew,
            CancellationToken cancellationToken = default)
        {
            var filter = Builders<NotificationEntity>.Filter.Eq(n => n.UserLogin, userLogin) &
                         Builders<NotificationEntity>.Filter.Ne(n => n.Status, NotificationStatus.Hidden);

            if (onlyNew)
            {
                filter &= Builders<NotificationEntity>.Filter.Eq(n => n.Status, NotificationStatus.Created);
            }

            return await _notificationCollection.Find(filter).ToListAsync(cancellationToken);
        }

        public async Task ChangeNotificationStatus(
            NotificationEntity notification,
            NotificationStatus status,
            CancellationToken cancellationToken = default
        )
        {
            var filter = Builders<NotificationEntity>.Filter.Eq(m => m.Id, notification.Id);
            var update = Builders<NotificationEntity>.Update.Set(m => m.Status, status);

            await _notificationCollection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
        }
    }
}
