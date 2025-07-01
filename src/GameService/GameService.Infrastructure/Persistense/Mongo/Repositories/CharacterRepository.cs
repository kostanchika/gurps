using GameService.Application.Features.Character.Queries.GetCharacters;
using GameService.Application.Interfaces.Repositories;
using GURPS.Character.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace GameService.Infrastructure.Persistense.Mongo.Repositories
{
    public class CharacterRepository : ICharacterRepository
    {
        private readonly IMongoCollection<CharacterEntity> _collection;

        public CharacterRepository(IMongoClient client, IOptions<MongoSettings> mongoSettings)
        {

            var database = client.GetDatabase(mongoSettings.Value.Database);
            _collection = database.GetCollection<CharacterEntity>(typeof(CharacterEntity).Name);
        }

        public Task AddAsync(CharacterEntity character, CancellationToken cancellationToken = default)
        {
            return _collection.InsertOneAsync(character, cancellationToken: cancellationToken);
        }

        public async Task<int> CountFilteredAsync(GetCharactersQuery filters, CancellationToken cancellationToken)
        {
            var builder = Builders<CharacterEntity>.Filter;
            var filter = builder.Empty;

            if (!string.IsNullOrWhiteSpace(filters.UserLogin))
                filter &= builder.Eq(x => x.UserLogin, filters.UserLogin);

            if (!string.IsNullOrWhiteSpace(filters.Name))
                filter &= builder.Regex(x => x.Name, new BsonRegularExpression(filters.Name, "i"));

            if (!string.IsNullOrWhiteSpace(filters.World))
                filter &= builder.Regex(x => x.World, new BsonRegularExpression(filters.World, "i"));

            if (!string.IsNullOrWhiteSpace(filters.Age))
                filter &= builder.Regex(x => x.Appearence.Age, new BsonRegularExpression(filters.World, "i"));

            if (!string.IsNullOrWhiteSpace(filters.BirthDate))
                filter &= builder.Regex(x => x.Appearence.BirthDate, new BsonRegularExpression(filters.World, "i"));

            if (!string.IsNullOrWhiteSpace(filters.Eyes))
                filter &= builder.Regex(x => x.Appearence.Eyes, new BsonRegularExpression(filters.World, "i"));

            if (!string.IsNullOrWhiteSpace(filters.Hair))
                filter &= builder.Regex(x => x.Appearence.Hair, new BsonRegularExpression(filters.World, "i"));

            if (!string.IsNullOrWhiteSpace(filters.Skin))
                filter &= builder.Regex(x => x.Appearence.Skin, new BsonRegularExpression(filters.World, "i"));

            if (!string.IsNullOrWhiteSpace(filters.Height))
                filter &= builder.Regex(x => x.Appearence.Height, new BsonRegularExpression(filters.World, "i"));

            if (!string.IsNullOrWhiteSpace(filters.Weight))
                filter &= builder.Regex(x => x.Appearence.Weight, new BsonRegularExpression(filters.World, "i"));

            if (!string.IsNullOrWhiteSpace(filters.Race))
                filter &= builder.Regex(x => x.Appearence.Race, new BsonRegularExpression(filters.World, "i"));

            if (!string.IsNullOrWhiteSpace(filters.Religion))
                filter &= builder.Regex(x => x.Appearence.Religion, new BsonRegularExpression(filters.World, "i"));

            return (int)await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
        }

        public async Task<IEnumerable<CharacterEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var cursor = await _collection.FindAsync(
                FilterDefinition<CharacterEntity>.Empty,
                cancellationToken: cancellationToken
            );

            return await cursor.ToListAsync(cancellationToken);
        }

        public async Task<CharacterEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var cursor = await _collection.FindAsync(
                Builders<CharacterEntity>.Filter.Eq(c => c.Id, id),
                cancellationToken: cancellationToken
            );

            return await cursor.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IEnumerable<CharacterEntity>> GetFilteredAsync(GetCharactersQuery filters, CancellationToken cancellationToken)
        {
            var builder = Builders<CharacterEntity>.Filter;
            var filter = builder.Empty;

            if (!string.IsNullOrWhiteSpace(filters.UserLogin))
                filter &= builder.Eq(x => x.UserLogin, filters.UserLogin);

            if (!string.IsNullOrWhiteSpace(filters.Name))
                filter &= builder.Regex(x => x.Name, new BsonRegularExpression(filters.Name, "i"));

            if (!string.IsNullOrWhiteSpace(filters.World))
                filter &= builder.Regex(x => x.World, new BsonRegularExpression(filters.World, "i"));

            if (!string.IsNullOrWhiteSpace(filters.Age))
                filter &= builder.Regex(x => x.Appearence.Age, new BsonRegularExpression(filters.World, "i"));

            if (!string.IsNullOrWhiteSpace(filters.BirthDate))
                filter &= builder.Regex(x => x.Appearence.BirthDate, new BsonRegularExpression(filters.World, "i"));

            if (!string.IsNullOrWhiteSpace(filters.Eyes))
                filter &= builder.Regex(x => x.Appearence.Eyes, new BsonRegularExpression(filters.World, "i"));

            if (!string.IsNullOrWhiteSpace(filters.Hair))
                filter &= builder.Regex(x => x.Appearence.Hair, new BsonRegularExpression(filters.World, "i"));

            if (!string.IsNullOrWhiteSpace(filters.Skin))
                filter &= builder.Regex(x => x.Appearence.Skin, new BsonRegularExpression(filters.World, "i"));

            if (!string.IsNullOrWhiteSpace(filters.Height))
                filter &= builder.Regex(x => x.Appearence.Height, new BsonRegularExpression(filters.World, "i"));

            if (!string.IsNullOrWhiteSpace(filters.Weight))
                filter &= builder.Regex(x => x.Appearence.Weight, new BsonRegularExpression(filters.World, "i"));

            if (!string.IsNullOrWhiteSpace(filters.Race))
                filter &= builder.Regex(x => x.Appearence.Race, new BsonRegularExpression(filters.World, "i"));

            if (!string.IsNullOrWhiteSpace(filters.Religion))
                filter &= builder.Regex(x => x.Appearence.Religion, new BsonRegularExpression(filters.World, "i"));

            var skip = (filters.Page - 1) * filters.PageSize;
            var limit = filters.PageSize;

            var cursor = _collection
                .Find(filter)
                .Skip(skip)
                .Limit(limit);

            return await cursor.ToListAsync(cancellationToken);
        }

        public Task RemoveAsync(CharacterEntity character, CancellationToken cancellationToken = default)
        {
            return _collection.DeleteOneAsync(
                Builders<CharacterEntity>.Filter.Eq(l => l.Id, character.Id),
                cancellationToken: cancellationToken
            );
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task UpdateAsync(CharacterEntity character, CancellationToken cancellationToken = default)
        {
            return _collection.ReplaceOneAsync(
                Builders<CharacterEntity>.Filter.Eq(l => l.Id, character.Id),
                character,
                cancellationToken: cancellationToken
            );
        }
    }
}
