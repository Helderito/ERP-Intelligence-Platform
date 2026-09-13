using ERP.Domain.MasterData;
using ERP.Domain.Tenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Infrastructure.Persistence.Configurations.MasterData;

public sealed class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
{
    public void Configure(EntityTypeBuilder<Warehouse> builder)
    {
        builder.ToTable("Warehouse");
        builder.HasKey(warehouse => warehouse.Id);
        builder.Ignore(warehouse => warehouse.DomainEvents);
        builder.Property(warehouse => warehouse.Id).ValueGeneratedNever();
        builder.Property(warehouse => warehouse.CompanyId).IsRequired();
        builder.HasIndex(warehouse => warehouse.CompanyId);
        builder.HasOne<Company>().WithMany().HasForeignKey(warehouse => warehouse.CompanyId).OnDelete(DeleteBehavior.Restrict);

        builder.OwnsOne(warehouse => warehouse.Code, codeBuilder =>
        {
            codeBuilder.Property(code => code.Value)
                .HasColumnName("Code")
                .HasMaxLength(50)
                .IsRequired();
        });

        builder.Property(warehouse => warehouse.Name).HasMaxLength(200).IsRequired();
        builder.Property(warehouse => warehouse.WarehouseTypeId).IsRequired();
        builder.Property(warehouse => warehouse.IsActive).IsRequired();
        builder.Property(warehouse => warehouse.CreatedAtUtc).IsRequired();
        builder.Property(warehouse => warehouse.UpdatedAtUtc);
        builder.Property(warehouse => warehouse.DeactivatedAtUtc);

        builder.HasOne(warehouse => warehouse.WarehouseType)
            .WithMany()
            .HasForeignKey(warehouse => warehouse.WarehouseTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
