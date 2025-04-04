using System.Linq.Expressions;
using UsersService.Domain.Entities;
using UsersService.Domain.Interfaces;

namespace UsersService.Application.Specifications.Auth
{
    public class UserByLoginSpecification(string login) : ISpecification<UserEntity>
    {
        public Expression<Func<UserEntity, bool>>? Criteria
            => user => user.Login == login;

        public List<Expression<Func<UserEntity, object>>>? Includes
            => null;
    }
}
