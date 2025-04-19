using System.Linq.Expressions;

namespace UsersService.Domain.Interfaces
{
    public interface ISpecification<T>
    {
        Expression<Func<T, bool>>? Criteria { get; }
        List<Expression<Func<T, object>>>? Includes { get; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }
    }

}
