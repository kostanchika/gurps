using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UsersService.Domain.Entities;

namespace UsersService.Infrastructure.Persistence.SQL.Configurations
{
    internal class FriendshipConfiguration : IEntityTypeConfiguration<FriendshipEntity>
    {
        public void Configure(EntityTypeBuilder<FriendshipEntity> builder)
        {
            builder.HasKey(u => u.Id);

            builder.HasOne(f => f.Initiator)
                .WithMany()
                .HasForeignKey(f => f.InitiatorId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(f => f.Recipent)
                .WithMany()
                .HasForeignKey(f => f.RecipentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(f => f.Status)
                .IsRequired()
                .HasConversion<byte>();

            builder.Property(f => f.CreatedAt)
                .HasDefaultValue(DateTime.UtcNow);

            builder.Property(f => f.UpdatedAt)
                .HasDefaultValue(DateTime.UtcNow);
        }
    }
}
