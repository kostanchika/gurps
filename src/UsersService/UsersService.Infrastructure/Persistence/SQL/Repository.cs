using Microsoft.EntityFrameworkCore;
using UsersService.Domain.Interfaces;

namespace UsersService.Infrastructure.Persistence.SQL
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly UsersContext _context;

        public Repository(UsersContext context)
        {
            _context = context;
        }

        public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _context.Set<T>().AddAsync(entity, cancellationToken);
        }

        public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Set<T>().ToListAsync(cancellationToken);
        }

        public async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Set<T>().FindAsync(id, cancellationToken);
        }

        public async Task<IEnumerable<T>> GetBySpecificationAsync(
            ISpecification<T> specification, 
            CancellationToken cancellationToken = default
        )
        {
            var query = _context.Set<T>().AsQueryable();

            if (specification.Criteria != null)
            {
                query = query.Where(specification.Criteria);
            }

            if (specification.Includes != null)
            {
                query = specification.Includes.Aggregate(query, (current, include) => current.Include(include));
            }

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<T?> GetOneBySpecificationAsync(
            ISpecification<T> specification, 
            CancellationToken cancellationToken = default
        )
        {
            var query = _context.Set<T>().AsQueryable();

            if (specification.Criteria != null)
            {
                query = query.Where(specification.Criteria);
            }

            if (specification.Includes != null)
            {
                query = specification.Includes.Aggregate(query, (current, include) => current.Include(include));
            }

            return await query.SingleOrDefaultAsync(cancellationToken);
        }

        public Task RemoveAsync(T entity, CancellationToken cancellationToken = default)
        {
            _context.Set<T>().Remove(entity);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            _context.Set<T>().Update(entity);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
