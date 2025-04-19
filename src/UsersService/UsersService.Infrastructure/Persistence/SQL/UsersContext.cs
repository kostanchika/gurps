using Microsoft.EntityFrameworkCore;
using UsersService.Infrastructure.Persistence.SQL.Configurations;

namespace UsersService.Infrastructure.Persistence.SQL
{
    public class UsersContext : DbContext
    {
        public UsersContext(DbContextOptions<UsersContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new FriendshipConfiguration());
        }
    }
}
