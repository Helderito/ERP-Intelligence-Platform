using ERP.Domain.MasterData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Infrastructure.Persistence.Configurations.MasterData;

public sealed class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.ToTable("Supplier");

        builder.HasKey(supplier => supplier.Id);

        builder.Ignore(supplier => supplier.DomainEvents);

        builder.Property(supplier => supplier.Id)
            .ValueGeneratedNever();

        builder.OwnsOne(supplier => supplier.Code, codeBuilder =>
        {
            codeBuilder.Property(code => code.Value)
                .HasColumnName("Code")
                .HasMaxLength(50)
                .IsRequired();

            codeBuilder.HasIndex(code => code.Value)
                .IsUnique();
        });

        builder.Property(supplier => supplier.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(supplier => supplier.IsActive)
            .IsRequired();

        builder.Property(supplier => supplier.CreatedAtUtc)
            .IsRequired();

        builder.Property(supplier => supplier.UpdatedAtUtc);

        builder.Property(supplier => supplier.DeactivatedAtUtc);

        builder.HasMany(supplier => supplier.Contacts)
            .WithOne()
            .HasForeignKey(contact => contact.SupplierId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(supplier => supplier.Contacts)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(supplier => supplier.Addresses)
            .WithOne()
            .HasForeignKey(address => address.SupplierId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(supplier => supplier.Addresses)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
