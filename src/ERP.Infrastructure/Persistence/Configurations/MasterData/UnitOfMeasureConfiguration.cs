using ERP.Domain.MasterData;
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

        builder.Property(unitOfMeasure => unitOfMeasure.Code)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(unitOfMeasure => unitOfMeasure.Code)
            .IsUnique();

        builder.Property(unitOfMeasure => unitOfMeasure.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasData(MasterDataSeed.UnitsOfMeasure);
    }
}
