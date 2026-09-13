using ERP.Domain.MasterData;
using ERP.Domain.Tenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Infrastructure.Persistence.Configurations.MasterData;

public sealed class TaxCodeConfiguration : IEntityTypeConfiguration<TaxCode>
{
    public void Configure(EntityTypeBuilder<TaxCode> builder)
    {
        builder.ToTable("TaxCode");
        builder.HasKey(taxCode => taxCode.Id);
        builder.Ignore(taxCode => taxCode.DomainEvents);
        builder.Property(taxCode => taxCode.Id).ValueGeneratedNever();
        builder.Property(taxCode => taxCode.CompanyId).IsRequired();
        builder.HasIndex(taxCode => taxCode.CompanyId);
        builder.HasOne<Company>().WithMany().HasForeignKey(taxCode => taxCode.CompanyId).OnDelete(DeleteBehavior.Restrict);
        builder.Property(taxCode => taxCode.Code).HasMaxLength(50).IsRequired();
        builder.HasIndex(taxCode => new { taxCode.CompanyId, taxCode.Code }).IsUnique();
        builder.Property(taxCode => taxCode.Name).HasMaxLength(100).IsRequired();
        builder.Property(taxCode => taxCode.Rate).HasPrecision(5, 2).IsRequired();
        builder.Property(taxCode => taxCode.IsActive).HasDefaultValue(true).IsRequired();
        builder.Property(taxCode => taxCode.CreatedAtUtc).IsRequired();
        builder.Property(taxCode => taxCode.UpdatedAtUtc);
        builder.Property(taxCode => taxCode.DeactivatedAtUtc);
    }
}
