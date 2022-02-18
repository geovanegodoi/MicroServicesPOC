using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;
using MSPOC.CrossCutting;

namespace MSPOC.CrossCutting.MongoDb
{
    public class MongoRepository<T> : IRepository<T> where T : Entity
    {
        protected readonly IMongoCollection<T> DbCollection;
        protected readonly FilterDefinitionBuilder<T> FilterBuilder = Builders<T>.Filter;
        
        public MongoRepository(IMongoDatabase database, string collectionName)
        {
            DbCollection = database.GetCollection<T>(collectionName);
        }

        public async Task<IReadOnlyCollection<T>> GetAllAsync()
            => await DbCollection.Find(FilterBuilder.Empty).ToListAsync();

        public async Task<T> GetAsync(Guid id)
        {
            var filter = FilterBuilder.Eq(entity => entity.Id, id);
            return await DbCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(T entity)
        {
            if (entity is null) throw new ArgumentNullException(nameof(entity));
            await DbCollection.InsertOneAsync(entity);
        }

        public async Task UpdateAsync(T entity)
        {
            if (entity is null) throw new ArgumentNullException(nameof(entity));

            var filter = FilterBuilder.Eq(entity => entity.Id, entity.Id);
            await DbCollection.ReplaceOneAsync(filter, entity);
        }

        public async Task RemoveAsync(T entity)
        {
            var filter = FilterBuilder.Eq(entity => entity.Id, entity.Id);
            await DbCollection.DeleteOneAsync(filter);
        }

        public async Task<IReadOnlyCollection<T>> FindAllAsync(Expression<Func<T, bool>> predicate)
            => await DbCollection.Find(predicate).ToListAsync();

        public async Task<T> FindAsync(Expression<Func<T, bool>> predicate)
            => await DbCollection.Find(predicate).FirstOrDefaultAsync();
    }
}