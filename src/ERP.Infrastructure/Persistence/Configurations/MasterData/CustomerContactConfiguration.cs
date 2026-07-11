using ERP.Domain.MasterData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Infrastructure.Persistence.Configurations.MasterData;

public sealed class CustomerContactConfiguration : IEntityTypeConfiguration<CustomerContact>
{
    public void Configure(EntityTypeBuilder<CustomerContact> builder)
    {
        builder.ToTable("CustomerContact");

        builder.HasKey(contact => contact.Id);

        builder.Ignore(contact => contact.DomainEvents);

        builder.Property(contact => contact.Id)
            .ValueGeneratedNever();

        builder.Property(contact => contact.CustomerId)
            .IsRequired();

        builder.Property(contact => contact.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(contact => contact.Email)
            .HasMaxLength(254);

        builder.Property(contact => contact.Phone)
            .HasMaxLength(50);
    }
}
