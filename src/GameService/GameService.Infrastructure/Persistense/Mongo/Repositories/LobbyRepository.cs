using GameService.Application.Interfaces.Repositories;
using GameService.Domain.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace GameService.Infrastructure.Persistense.Mongo.Repositories
{
    public class LobbyRepository : ILobbyRepository
    {
        private readonly IMongoCollection<LobbyEntity> _collection;

        public LobbyRepository(IMongoClient client, IOptions<MongoSettings> mongoSettings)
        {
            var database = client.GetDatabase(mongoSettings.Value.Database);
            _collection = database.GetCollection<LobbyEntity>(typeof(LobbyEntity).Name);
        }

        public Task AddAsync(LobbyEntity lobby, CancellationToken cancellationToken = default)
        {
            return _collection.InsertOneAsync(lobby, cancellationToken: cancellationToken);
        }

        public async Task<int> CountFilteredAsync(
            bool? isPublic,
            int? playersCountFrom,
            int? playersCountTo,
            CancellationToken cancellationToken = default
        )
        {
            var builder = Builders<LobbyEntity>.Filter;
            var filter = builder.Empty;

            if (isPublic.HasValue)
            {
                if (isPublic.Value)
                {
                    filter &= builder.Eq(l => string.IsNullOrEmpty(l.Password), true);
                }
                else
                {
                    filter &= builder.Eq(l => string.IsNullOrEmpty(l.Password), false);
                }
            }

            if (playersCountFrom.HasValue)
            {
                filter &= builder.Gte(x => x.Players.Count, playersCountFrom.Value);
            }

            if (playersCountTo.HasValue)
            {
                filter &= builder.Lte(x => x.Players.Count, playersCountTo.Value);
            }

            var result = await _collection
                .CountDocumentsAsync(filter, cancellationToken: cancellationToken);

            return (int)result;
        }

        public async Task<LobbyEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var cursor = await _collection.FindAsync(
                Builders<LobbyEntity>.Filter.Eq(l => l.Id, id),
                cancellationToken: cancellationToken
            );

            return await cursor.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<LobbyEntity?> GetByUserLoginAsync(string login, CancellationToken cancellationToken = default)
        {
            var cursor = await _collection.FindAsync(
                Builders<LobbyEntity>.Filter.ElemMatch(l => l.Players, p => p.Login == login),
                cancellationToken: cancellationToken
            );

            return await cursor.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IEnumerable<LobbyEntity>> GetFilteredAsync(
            bool? isPublic,
            int? playersCountFrom,
            int? playersCountTo,
            int? page,
            int? pageSize,
            CancellationToken cancellationToken = default
        )
        {
            var builder = Builders<LobbyEntity>.Filter;
            var filter = builder.Empty;

            if (isPublic.HasValue)
            {
                if (isPublic.Value)
                {
                    filter &= builder.Eq(l => string.IsNullOrEmpty(l.Password), true);
                }
                else
                {
                    filter &= builder.Eq(l => string.IsNullOrEmpty(l.Password), false);
                }
            }

            if (playersCountFrom.HasValue)
            {
                filter &= builder.Gte(x => x.Players.Count, playersCountFrom.Value);
            }

            if (playersCountTo.HasValue)
            {
                filter &= builder.Lte(x => x.Players.Count, playersCountTo.Value);
            }

            var skip = (page.GetValueOrDefault(1) - 1) * pageSize.GetValueOrDefault(10);
            var limit = pageSize.GetValueOrDefault(10);

            var result = await _collection
                .Find(filter)
                .Skip(skip)
                .Limit(limit)
                .ToListAsync(cancellationToken);

            return result;
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task UpdateAsync(LobbyEntity lobby, CancellationToken cancellationToken = default)
        {
            return _collection.ReplaceOneAsync(
                Builders<LobbyEntity>.Filter.Eq(l => l.Id, lobby.Id),
                lobby,
                cancellationToken: cancellationToken
            );
        }

        public Task RemoveAsync(LobbyEntity lobby, CancellationToken cancellationToken = default)
        {
            return _collection.DeleteOneAsync(
                Builders<LobbyEntity>.Filter.Eq(l => l.Id, lobby.Id),
                cancellationToken: cancellationToken
            );
        }
    }
}
