using ERP.Domain.MasterData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Infrastructure.Persistence.Configurations.MasterData;

public sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customer");

        builder.HasKey(customer => customer.Id);

        builder.Ignore(customer => customer.DomainEvents);

        builder.Property(customer => customer.Id)
            .ValueGeneratedNever();

        builder.OwnsOne(customer => customer.Code, codeBuilder =>
        {
            codeBuilder.Property(code => code.Value)
                .HasColumnName("Code")
                .HasMaxLength(50)
                .IsRequired();

            codeBuilder.HasIndex(code => code.Value)
                .IsUnique();
        });

        builder.Property(customer => customer.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(customer => customer.IsActive)
            .IsRequired();

        builder.Property(customer => customer.CreatedAtUtc)
            .IsRequired();

        builder.Property(customer => customer.UpdatedAtUtc);

        builder.Property(customer => customer.DeactivatedAtUtc);

        builder.HasMany(customer => customer.Contacts)
            .WithOne()
            .HasForeignKey(contact => contact.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(customer => customer.Contacts)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(customer => customer.Addresses)
            .WithOne()
            .HasForeignKey(address => address.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(customer => customer.Addresses)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
