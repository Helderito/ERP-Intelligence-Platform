using ERP.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Infrastructure.Persistence.Configurations;

public sealed class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("UserRole");

        builder.HasKey(userRole => userRole.Id);

        builder.Ignore(userRole => userRole.DomainEvents);

        builder.Property(userRole => userRole.Id)
            .ValueGeneratedNever();

        builder.Property(userRole => userRole.UserId)
            .IsRequired();

        builder.Property(userRole => userRole.RoleId)
            .IsRequired();

        builder.Property(userRole => userRole.AssignedAtUtc)
            .IsRequired();

        builder.HasIndex(userRole => new { userRole.UserId, userRole.RoleId })
            .IsUnique();

        builder.HasOne<Role>()
            .WithMany()
            .HasForeignKey(userRole => userRole.RoleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
