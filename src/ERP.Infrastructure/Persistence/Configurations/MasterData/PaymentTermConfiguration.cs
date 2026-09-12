using ERP.Domain.MasterData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Infrastructure.Persistence.Configurations.MasterData;

public sealed class PaymentTermConfiguration : IEntityTypeConfiguration<PaymentTerm>
{
    public void Configure(EntityTypeBuilder<PaymentTerm> builder)
    {
        builder.ToTable("PaymentTerm");
        builder.HasKey(term => term.Id);
        builder.Ignore(term => term.DomainEvents);
        builder.Property(term => term.Id).ValueGeneratedNever();
        builder.Property(term => term.Code).HasMaxLength(50).IsRequired();
        builder.HasIndex(term => term.Code).IsUnique();
        builder.Property(term => term.Name).HasMaxLength(100).IsRequired();
        builder.Property(term => term.NetDays).IsRequired();
        builder.HasData(MasterDataSeed.PaymentTerms);
    }
}
