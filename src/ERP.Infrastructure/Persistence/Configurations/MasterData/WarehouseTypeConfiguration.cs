using ERP.Domain.MasterData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Infrastructure.Persistence.Configurations.MasterData;

public sealed class WarehouseTypeConfiguration : IEntityTypeConfiguration<WarehouseType>
{
    public void Configure(EntityTypeBuilder<WarehouseType> builder)
    {
        builder.ToTable("WarehouseType");
        builder.HasKey(type => type.Id);
        builder.Ignore(type => type.DomainEvents);
        builder.Property(type => type.Id).ValueGeneratedNever();
        builder.Property(type => type.Code).HasMaxLength(50).IsRequired();
        builder.HasIndex(type => type.Code).IsUnique();
        builder.Property(type => type.Name).HasMaxLength(100).IsRequired();
        builder.HasData(MasterDataSeed.WarehouseTypes);
    }
}
