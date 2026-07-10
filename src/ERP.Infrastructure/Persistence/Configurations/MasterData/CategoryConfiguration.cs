using ERP.Domain.MasterData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Infrastructure.Persistence.Configurations.MasterData;

public sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Category");

        builder.HasKey(category => category.Id);

        builder.Ignore(category => category.DomainEvents);

        builder.Property(category => category.Id)
            .ValueGeneratedNever();

        builder.Property(category => category.Code)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(category => category.Code)
            .IsUnique();

        builder.Property(category => category.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasData(MasterDataSeed.Categories);
    }
}
