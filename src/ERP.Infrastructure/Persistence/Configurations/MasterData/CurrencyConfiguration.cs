using ERP.Domain.MasterData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Infrastructure.Persistence.Configurations.MasterData;

public sealed class CurrencyConfiguration : IEntityTypeConfiguration<Currency>
{
    public void Configure(EntityTypeBuilder<Currency> builder)
    {
        builder.ToTable("Currency");
        builder.HasKey(currency => currency.Id);
        builder.Ignore(currency => currency.DomainEvents);
        builder.Property(currency => currency.Id).ValueGeneratedNever();
        builder.Property(currency => currency.Code).HasMaxLength(3).IsRequired();
        builder.HasIndex(currency => currency.Code).IsUnique();
        builder.Property(currency => currency.Name).HasMaxLength(100).IsRequired();
        builder.HasData(MasterDataSeed.Currencies);
    }
}
