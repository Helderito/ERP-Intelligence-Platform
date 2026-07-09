using ERP.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Infrastructure.Persistence.Configurations;

public sealed class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("Permission");

        builder.HasKey(permission => permission.Id);

        builder.Ignore(permission => permission.DomainEvents);

        builder.Property(permission => permission.Id)
            .ValueGeneratedNever();

        builder.Property(permission => permission.Code)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(permission => permission.Code)
            .IsUnique();

        builder.Property(permission => permission.Description)
            .HasMaxLength(250)
            .IsRequired();

        builder.HasData(PermissionSeed.Permissions);
    }
}
