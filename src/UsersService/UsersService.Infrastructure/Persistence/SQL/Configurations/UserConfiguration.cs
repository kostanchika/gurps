using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UsersService.Domain.Entities;

namespace UsersService.Infrastructure.Persistence.SQL.Configurations
{
    internal class UserConfiguration : IEntityTypeConfiguration<UserEntity>
    {
        public void Configure(EntityTypeBuilder<UserEntity> builder)
        {
            builder.HasKey(u => u.Id);

            builder.Property(u => u.Login)
                .IsRequired()
                .HasMaxLength(30);

            builder.HasIndex(u => u.Login)
                .IsUnique();

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(254);

            builder.HasIndex(u => u.Email)
                .IsUnique();

            builder.Property(u => u.PasswordHash)
                .IsRequired();

            builder.Property(u => u.Role)
                .IsRequired();

            builder.Property(u => u.CreatedAt)
                .HasDefaultValue(DateTime.UtcNow);
        }
    }
}
