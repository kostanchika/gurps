using System.Linq.Expressions;
using UsersService.Domain.Entities;
using UsersService.Domain.Interfaces;

namespace UsersService.Application.Specifications.Auth
{
    public class UserByEmailSpecification(string email) : ISpecification<UserEntity>
    {
        public Expression<Func<UserEntity, bool>>? Criteria
            => user => user.Email == email;

        public List<Expression<Func<UserEntity, object>>>? Includes
            => null;

        public int? Page { get; set; }
        public int? PageSize { get; set; }
    }
}
