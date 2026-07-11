using ERP.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Infrastructure.Persistence.Configurations;

public sealed class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("RolePermission");

        builder.HasKey(rolePermission => rolePermission.Id);

        builder.Ignore(rolePermission => rolePermission.DomainEvents);

        builder.Property(rolePermission => rolePermission.Id)
            .ValueGeneratedNever();

        builder.Property(rolePermission => rolePermission.RoleId)
            .IsRequired();

        builder.Property(rolePermission => rolePermission.PermissionId)
            .IsRequired();

        builder.Property(rolePermission => rolePermission.AssignedAtUtc)
            .IsRequired();

        builder.HasIndex(rolePermission => new { rolePermission.RoleId, rolePermission.PermissionId })
            .IsUnique();

        builder.HasOne<Permission>()
            .WithMany()
            .HasForeignKey(rolePermission => rolePermission.PermissionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(
            new
            {
                Id = RoleSeed.AdministratorRolesManageLinkId,
                RoleId = IdentitySeed.AdministratorRoleId,
                PermissionId = PermissionSeed.RolesManageId,
                AssignedAtUtc = RoleSeed.SeededAtUtc
            },
            new
            {
                Id = RoleSeed.AdministratorUsersManageLinkId,
                RoleId = IdentitySeed.AdministratorRoleId,
                PermissionId = PermissionSeed.UsersManageId,
                AssignedAtUtc = RoleSeed.SeededAtUtc
            },
            new
            {
                Id = RoleSeed.AdministratorCatalogManageLinkId,
                RoleId = IdentitySeed.AdministratorRoleId,
                PermissionId = PermissionSeed.CatalogManageId,
                AssignedAtUtc = RoleSeed.SeededAtUtc
            },
            new
            {
                Id = RoleSeed.AdministratorCustomersManageLinkId,
                RoleId = IdentitySeed.AdministratorRoleId,
                PermissionId = PermissionSeed.CustomersManageId,
                AssignedAtUtc = RoleSeed.SeededAtUtc
            });
    }
}
