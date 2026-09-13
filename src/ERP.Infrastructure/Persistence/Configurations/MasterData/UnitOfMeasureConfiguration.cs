using ERP.Domain.MasterData;
using ERP.Domain.Tenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Infrastructure.Persistence.Configurations.MasterData;

public sealed class UnitOfMeasureConfiguration : IEntityTypeConfiguration<UnitOfMeasure>
{
    public void Configure(EntityTypeBuilder<UnitOfMeasure> builder)
    {
        builder.ToTable("UnitOfMeasure");

        builder.HasKey(unitOfMeasure => unitOfMeasure.Id);

        builder.Ignore(unitOfMeasure => unitOfMeasure.DomainEvents);

        builder.Property(unitOfMeasure => unitOfMeasure.Id)
            .ValueGeneratedNever();

        builder.Property(unitOfMeasure => unitOfMeasure.CompanyId).IsRequired();
        builder.HasIndex(unitOfMeasure => unitOfMeasure.CompanyId);
        builder.HasOne<Company>().WithMany().HasForeignKey(unit => unit.CompanyId).OnDelete(DeleteBehavior.Restrict);

        builder.Property(unitOfMeasure => unitOfMeasure.Code)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(unitOfMeasure => new { unitOfMeasure.CompanyId, unitOfMeasure.Code })
            .IsUnique();

        builder.Property(unitOfMeasure => unitOfMeasure.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(unitOfMeasure => unitOfMeasure.IsActive)
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(unitOfMeasure => unitOfMeasure.CreatedAtUtc).IsRequired();
        builder.Property(unitOfMeasure => unitOfMeasure.UpdatedAtUtc);
        builder.Property(unitOfMeasure => unitOfMeasure.DeactivatedAtUtc);

        builder.HasData(MasterDataSeed.UnitsOfMeasure);
    }
}
