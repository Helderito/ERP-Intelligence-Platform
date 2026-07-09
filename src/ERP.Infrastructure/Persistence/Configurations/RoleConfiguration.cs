using ERP.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Infrastructure.Persistence.Configurations;

public sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Role");

        builder.HasKey(role => role.Id);

        builder.Ignore(role => role.DomainEvents);

        builder.Property(role => role.Id)
            .ValueGeneratedNever();

        builder.Property(role => role.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(role => role.Name)
            .IsUnique();

        builder.Property(role => role.IsActive)
            .IsRequired();

        builder.Property(role => role.CreatedAtUtc)
            .IsRequired();

        builder.Property(role => role.UpdatedAtUtc);

        builder.Property(role => role.DeactivatedAtUtc);

        builder.HasMany<RolePermission>("_rolePermissions")
            .WithOne()
            .HasForeignKey(rolePermission => rolePermission.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation("_rolePermissions")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasData(new
        {
            Id = IdentitySeed.AdministratorRoleId,
            Name = IdentitySeed.AdministratorRoleName,
            IsActive = true,
            CreatedAtUtc = RoleSeed.SeededAtUtc
        });
    }
}
