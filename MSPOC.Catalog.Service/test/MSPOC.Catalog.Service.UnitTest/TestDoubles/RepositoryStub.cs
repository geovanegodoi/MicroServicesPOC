using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MSPOC.CrossCutting;

namespace MSPOC.Catalog.Service.UnitTest.TestDoubles
{
    public class RepositoryStub<T> : IRepository<T> where T : Entity
    {
        private readonly Dictionary<Guid, T> _inMemoryDb = new ();

        public Task CreateAsync(T entity)
            => Task.Run(() => _inMemoryDb[entity.Id] = entity);

        public Task<IReadOnlyCollection<T>> FindAllAsync(Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<T> FindAsync(Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<T>> GetAllAsync()
            => Task.Run(() => (IReadOnlyCollection<T>)_inMemoryDb.ToList());

        public Task<T> GetAsync(Guid id)
            => Task.Run<T>(() => _inMemoryDb[id]);

        public Task RemoveAsync(T entity)
            => Task.Run(() => _inMemoryDb.Remove(entity.Id));

        public Task UpdateAsync(T entity)
            => Task.Run(() => _inMemoryDb[entity.Id] = entity);
    }
}