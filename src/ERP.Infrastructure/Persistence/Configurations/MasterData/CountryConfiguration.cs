using ERP.Domain.MasterData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Infrastructure.Persistence.Configurations.MasterData;

public sealed class CountryConfiguration : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.ToTable("Country");
        builder.HasKey(country => country.Id);
        builder.Ignore(country => country.DomainEvents);
        builder.Property(country => country.Id).ValueGeneratedNever();
        builder.Property(country => country.Code).HasMaxLength(2).IsRequired();
        builder.HasIndex(country => country.Code).IsUnique();
        builder.Property(country => country.Name).HasMaxLength(100).IsRequired();
        builder.HasData(MasterDataSeed.Countries);
    }
}
