using GURPS.Character.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using UsersService.Domain.Interfaces;
using UsersService.Infrastructure.Persistence.NoSQL.Configurations;

namespace UsersService.Infrastructure.Persistence.NoSQL
{
    public class CharacterRepository : IRepository<CharacterEntity>
    {
        private readonly IMongoCollection<CharacterEntity> _characterCollection;
        private readonly IMongoCollection<Counter> _counterCollection;

        public CharacterRepository(IMongoClient client, IOptions<MongoSettings> mongoSettings)
        {
            var database = client.GetDatabase(mongoSettings.Value.Database);
            _characterCollection = database.GetCollection<CharacterEntity>(typeof(CharacterEntity).Name);
            _counterCollection = database.GetCollection<Counter>(typeof(Counter).Name);
        }

        public async Task<IEnumerable<CharacterEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _characterCollection.Find(_ => true).ToListAsync(cancellationToken);
        }

        public async Task<CharacterEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _characterCollection.Find(character => character.Id == id)
                    .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<int> GetCountBySpecificationAsync(
            ISpecification<CharacterEntity> specification, 
            CancellationToken cancellationToken = default
        )
        {
            var filter = specification.Criteria != null
                ? Builders<CharacterEntity>.Filter.Where(specification.Criteria)
                : Builders<CharacterEntity>.Filter.Empty;


            return (int)await _characterCollection.CountDocumentsAsync(
                filter,
                null,
                cancellationToken
            );
        }

        public async Task<IEnumerable<CharacterEntity>> GetBySpecificationAsync(
            ISpecification<CharacterEntity> specification, 
            CancellationToken cancellationToken = default
        )
        {
            var filter = specification.Criteria != null
                ? Builders<CharacterEntity>.Filter.Where(specification.Criteria)
                : Builders<CharacterEntity>.Filter.Empty;

            var query = _characterCollection.Find(filter);

            if (specification.Page != null && specification.PageSize != null)
            {
                query = query.Skip((specification.Page.Value - 1) * specification.PageSize.Value)
                             .Limit(specification.PageSize.Value);
            }


            return await query.ToListAsync(cancellationToken);
        }

        public async Task<CharacterEntity?> GetOneBySpecificationAsync(
            ISpecification<CharacterEntity> specification, 
            CancellationToken cancellationToken = default
        )
        {
            var filter = specification.Criteria != null
                ? Builders<CharacterEntity>.Filter.Where(specification.Criteria)
                : Builders<CharacterEntity>.Filter.Empty;

            return await _characterCollection.Find(filter)
                    .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task AddAsync(CharacterEntity entity, CancellationToken cancellationToken = default)
        {
            var filter = Builders<Counter>.Filter.Eq(counter => counter.Name, "CharacterId");
            var update = Builders<Counter>.Update.Inc(counter => counter.Value, 1);
            var options = new FindOneAndUpdateOptions<Counter>
            {
                IsUpsert = true,
                ReturnDocument = ReturnDocument.After
            };
            var counter = await _counterCollection.FindOneAndUpdateAsync(filter, update, options, cancellationToken);
        
            entity.Id = counter.Value;
            await _characterCollection.InsertOneAsync(entity, null, cancellationToken);
        }

        public async Task UpdateAsync(CharacterEntity entity, CancellationToken cancellationToken = default)
        {
            await _characterCollection.ReplaceOneAsync(
                character => character.Id == entity.Id, 
                entity, 
                cancellationToken: cancellationToken
            );
        }

        public async Task RemoveAsync(CharacterEntity entity, CancellationToken cancellationToken = default)
        {
            await _characterCollection.DeleteOneAsync(
                character => character.Id == entity.Id, 
                cancellationToken
            );
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}
