using ERP.Domain.MasterData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Infrastructure.Persistence.Configurations.MasterData;

public sealed class SupplierAddressConfiguration : IEntityTypeConfiguration<SupplierAddress>
{
    public void Configure(EntityTypeBuilder<SupplierAddress> builder)
    {
        builder.ToTable("SupplierAddress");

        builder.HasKey(address => address.Id);

        builder.Ignore(address => address.DomainEvents);

        builder.Property(address => address.Id)
            .ValueGeneratedNever();

        builder.Property(address => address.SupplierId)
            .IsRequired();

        builder.Property(address => address.Line1)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(address => address.Line2)
            .HasMaxLength(200);

        builder.Property(address => address.City)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(address => address.PostalCode)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(address => address.Country)
            .HasMaxLength(100)
            .IsRequired();
    }
}
