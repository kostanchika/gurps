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

        public async Task AddAsync(T entity, CancellationToken ct = default)
        {
            await _context.Set<T>().AddAsync(entity, ct);
        }

        public async Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default)
        {
            return await _context.Set<T>().ToListAsync(ct);
        }

        public async Task<T?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _context.Set<T>().FindAsync(id, ct);
        }

        public async Task<IEnumerable<T>> GetBySpecificationAsync(ISpecification<T> specification, CancellationToken ct = default)
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

            return await query.ToListAsync(ct);
        }

        public async Task<T?> GetOneBySpecificationAsync(ISpecification<T> specification, CancellationToken ct = default)
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

            return await query.SingleOrDefaultAsync(ct);
        }

        public Task RemoveAsync(T entity, CancellationToken ct = default)
        {
            _context.Set<T>().Remove(entity);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(T entity, CancellationToken ct = default)
        {
            _context.Set<T>().Update(entity);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _context.SaveChangesAsync(ct);
        }
    }
}
